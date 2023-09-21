namespace EdcHost.Games;

/// <summary>
/// Game handles the game logic.
/// </summary>
public interface IGame
{
    /// <summary>
    /// Stage of game.
    /// </summary>
    public enum Stage
    {
        Ready,
        Running,
        Battling,
        Finished
    }

    /// <summary>
    /// Current stage of the game.
    /// </summary>
    public Stage CurrentStage { get; }

    /// <summary>
    /// Elapsed time of the game.
    /// </summary>
    public TimeSpan ElapsedTime { get; }

    /// <summary>
    /// Winner of the game.
    /// </summary>
    /// <remarks>
    /// Winner can be null in case there is no winner.
    /// </remarks>
    public IPlayer? Winner { get; }

    /// <summary>
    /// Ticks the game.
    /// </summary>
    public void Tick();
}
