using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fleck;
using Serilog;
using Serilog.Core;
using System.Diagnostics;
namespace EdcHost.ViewerServers;

/// <summary>
/// ViewerServer handles the API requests from viewers via WebSocket.
/// </summary>
public class ViewerServer
{
    private readonly WebSocketServer _webSocketServer;
    private readonly ILogger _logger = Log.Logger.ForContext<ViewerServer>();
    private IWebSocketConnection? _socket = null;

    public ViewerServer(int port)
    {
        _webSocketServer = new WebSocketServer("ws://localhost:" + port)
        {
            RestartAfterListenError = true
        };
    }
    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        WebSocketServerStart();
        _logger.Information("Server started.");

        
    }
    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        _webSocketServer.Dispose();
        _socket?.Close();
    }

    public void Send(IMessage message)
    {
        try
        {
            byte[] bytes = message.SerializeToUtf8Bytes();
            if (_socket == null)
            {
                throw new Exception("Socket not specified.");
            }
            _socket?.Send(bytes);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while sending message.");
        }
    }

    public void WebSocketServerStart()
    {
        _webSocketServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _logger.Debug("WebSocket connection opened.");
                if (_socket == null)
                {
                    _socket = socket;
                }
            };

            socket.OnClose = () =>
            {
                _logger.Debug("WebSocket connection closed.");
            };

            socket.OnMessage = text =>
            {
                try
                {
                    IMessage message = JsonSerializer.Deserialize<Message>(text)!;
                    switch (message.MessageType)
                    {
                        case "COMPETITION_CONTROL_COMMAND":
                            ICompetitionControlCommand command 
                                = JsonSerializer.Deserialize<CompetitionControlCommand>(text)!;
                            break;
                        
                        case "HOST_CONFIGURATION_FROM_CLIENT":
                            IHostConfigurationFromClient hostConfiguration
                                = JsonSerializer.Deserialize<HostConfigurationFromClient>(text)!;
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while parsing message.");
                    socket.Close();
                }
            };

            socket.OnBinary = bytes =>
            {
                try
                {
                    string text = Encoding.UTF8.GetString(bytes);
                    IMessage message = JsonSerializer.Deserialize<Message>(text)!;
                    switch (message.MessageType)
                    {
                        case "COMPETITION_CONTROL_COMMAND":
                            ICompetitionControlCommand command 
                                = JsonSerializer.Deserialize<CompetitionControlCommand>(text)!;
                            break;
                        
                        case "HOST_CONFIGURATION_FROM_CLIENT":
                            IHostConfigurationFromClient hostConfiguration
                                = JsonSerializer.Deserialize<HostConfigurationFromClient>(text)!;
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while parsing message.");
                    socket.Close();
                }
            };

            socket.OnError = exception => 
            {
                _logger.Error(exception, "Error while receiving message.");
                socket.Close();
            };
        });
    }
}
