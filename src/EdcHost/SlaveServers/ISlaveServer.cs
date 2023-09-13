namespace EdcHost.SlaveServers;

/// <summary>
/// SlaveServer handles the communication with the slaves via UART.
/// </summary>
public interface ISlaveServer
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
