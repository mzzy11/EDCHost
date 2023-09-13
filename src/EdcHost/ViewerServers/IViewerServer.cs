namespace EdcHost.ViewerServers;

/// <summary>
/// ViewerServer handles the API requests from viewers via WebSocket.
/// </summary>
public interface IViewerServer
{
    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start();
    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop();
}
