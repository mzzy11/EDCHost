namespace EdcHost;

/// <summary>
/// EdcHost does all the work of the program.
/// </summary>
public interface IEdcHost
{
    /// <summary>
    /// Starts the host.
    /// </summary>
    public void Start();

    /// <summary>
    /// Stops the host.
    /// </summary>
    public void Stop();
}
