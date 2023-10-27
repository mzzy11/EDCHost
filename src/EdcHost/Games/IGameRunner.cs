namespace EdcHost.Games;

public interface IGameRunner : IDisposable
{
    static IGameRunner Create(IGame game)
    {
        return new GameRunner(game);
    }

    IGame Game { get; }
    void Start();
    void End();
}
