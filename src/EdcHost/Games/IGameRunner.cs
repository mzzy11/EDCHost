namespace EdcHost.Games;

public interface IGameRunner
{
    static IGameRunner Create(IGame game)
    {
        return new GameRunner(game);
    }

    IGame Game { get; }
    void Start();
    void End();

    bool IsRunning { get; }
}
