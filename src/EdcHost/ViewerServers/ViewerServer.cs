using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Fleck;
using Serilog;

namespace EdcHost.ViewerServers;

/// <summary>
/// ViewerServer handles the API requests from viewers via WebSocket.
/// </summary>
public class ViewerServer : IViewerServer
{
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    readonly ILogger _logger = Log.Logger.ForContext("Component", "ViewerServers");
    readonly ConcurrentQueue<Message> _messagesToSend = new();
    readonly int _port;
    readonly ConcurrentDictionary<Guid, IWebSocketConnection> _sockets = new();
    readonly IWebSocketServerHub _wsServerHub;

    bool _isRunning = false;
    Task? _task = null;
    CancellationTokenSource? _taskCancellationTokenSource = null;
    IWebSocketServer? _wsServer = null;


    public ViewerServer(int port, IWebSocketServerHub wsServerHub)
    {
        _port = port;
        _wsServerHub = wsServerHub;
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("already running");
        }

        Debug.Assert(_task is null);
        Debug.Assert(_taskCancellationTokenSource is null);
        Debug.Assert(_wsServer is null);

        _logger.Information("Starting...");

        _wsServer = _wsServerHub.Get(8080);
        StartWsServer();

        _taskCancellationTokenSource = new();
        _task = Task.Run(TaskForSendingFunc);

        _isRunning = true;

        _logger.Information("Started.");
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        if (!_isRunning)
        {
            throw new InvalidOperationException("not running");
        }

        _logger.Information("Stopping...");

        Debug.Assert(_task is not null);
        Debug.Assert(_taskCancellationTokenSource is not null);
        Debug.Assert(_wsServer is not null);

        _taskCancellationTokenSource.Cancel();
        _task.Wait();
        _taskCancellationTokenSource.Dispose();
        _task.Dispose();

        _wsServer.Dispose();

        _task = null;
        _taskCancellationTokenSource = null;
        _wsServer = null;

        _isRunning = false;

        _logger.Information("Stopped.");
    }

    public void Publish(Message message)
    {
        _messagesToSend.Enqueue(message);
    }

    void ParseMessage(string text)
    {
        Message? generalMessage = JsonSerializer.Deserialize<Message>(text) ?? throw new Exception("failed to deserialize message");

        switch (generalMessage.MessageType)
        {
            case "COMPETITION_CONTROL_COMMAND":
                AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                    JsonSerializer.Deserialize<CompetitionControlCommandMessage>(text)
                    ?? throw new Exception("failed to deserialize CompetitionControlCommandMessage")
                ));
                break;

            case "HOST_CONFIGURATION_FROM_CLIENT":
                AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                    JsonSerializer.Deserialize<HostConfigurationFromClientMessage>(text)
                    ?? throw new Exception("failed to deserialize HostConfigurationFromClientMessage")
                ));
                break;

            default:
                throw new Exception($"invalid message type: {generalMessage.MessageType}");
        }
    }


    void StartWsServer()
    {
        Debug.Assert(_wsServer is not null);

        _wsServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _logger.Debug("Connection from {ClientIpAddress} opened.", socket.ConnectionInfo.ClientIpAddress);

                // Remove the socket if it already exists.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);

                // Add the socket.
                _sockets.TryAdd(socket.ConnectionInfo.Id, socket);
            };

            socket.OnClose = () =>
            {
                _logger.Debug("Connection from {ClientIpAddress} closed.", socket.ConnectionInfo.ClientIpAddress);

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };

            socket.OnMessage = text =>
            {
                try
                {
                    ParseMessage(text);
                }
                catch (Exception)
                {
                    _logger.Error($"Failed to parse message: {text}");

#if DEBUG
                    throw;
#endif
                }
            };

            socket.OnBinary = bytes =>
            {
                try
                {
                    string text = Encoding.UTF8.GetString(bytes);
                    ParseMessage(text);
                }
                catch (Exception)
                {
                    _logger.Error($"Failed to parse message: {bytes}");

#if DEBUG
                    throw;
#endif
                }
            };

            socket.OnError = exception =>
            {
#if DEBUG
                _logger.Error(exception, "Socket error."); // Even in DEBUG, we don't throw the exception.
#else
                _logger.Error("Socket error.");
#endif

                // Close the socket.
                socket.Close();

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };
        });
    }

    void TaskForSendingFunc()
    {
        _logger.Debug("SendTaskFunc started");

        while (true)
        {
            try
            {
                if (_messagesToSend.TryDequeue(out Message? message))
                {
                    string jsonString = message.Json;

                    foreach (IWebSocketConnection socket in _sockets.Values)
                    {
                        socket.Send(jsonString);
                    }
                }
            }
            catch (Exception)
            {
                _logger.Error("Error while sending message.");

#if DEBUG
                throw;
#endif
            }
        }
    }

}
