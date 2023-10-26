using EdcHost.ViewerServers.EventArgs;
using Fleck;

namespace EdcHost.ViewerServers;

/// <summary>
/// ViewerServer handles the API requests from viewers via WebSocket.
/// </summary>
public interface IViewerServer
{
    static IViewerServer Create(int port)
    {
        var webSocketServer = new WebSocketServer($"ws://0.0.0.0:{port}");
        return new ViewerServer(webSocketServer, new Updater(), new GameController());
    }

    IUpdater CompetitionUpdater { get; }
    IGameController Controller { get; }
    event EventHandler<SetPortEventArgs>? SetPortEvent;
    event EventHandler<SetCameraEventArgs>? SetCameraEvent;

    /// <summary>
    /// Starts the server.
    /// </summary>
    void Start();
    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();
    /// <summary>
    /// Raise an error to the viewer.
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="message"></param>
    void RaiseError(int errorCode, string message);
}
