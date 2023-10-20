using EdcHost.Games;

namespace EdcHost;

/// <summary>
/// EdcHost does all the work of the program.
/// </summary>
public interface IEdcHost
{
    static IEdcHost Create(EdcHostOptions options)
    {
        var game = IGame.Create();
        var gameRunner = IGameRunner.Create(game);
        SlaveServers.SlaveServer slaveServer = new(new string[] { }, new int[] { });
        ViewerServers.ViewerServer viewerServer = new(options.ServerPort);

        return new EdcHost(
            game: game,
            gameRunner: gameRunner,
            slaveServer: slaveServer,
            viewerServer: viewerServer
        );
    }

    /// <summary>
    /// Starts the host.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the host.
    /// </summary>
    void Stop();
}
