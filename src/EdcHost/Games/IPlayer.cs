namespace EdcHost.Games;

/// <summary>
/// Player represents a player in the game.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// The kind of a commodity.
    /// </summary>
    public enum CommodityKindType
    {
        AgilityBoost,
        HealthBoost,
        HealthPotion,
        StrengthBoost,
        Wool,
    }

    /// <summary>
    /// The kind of an action.
    /// </summary>
    public enum ActionKindType
    {
        Attack,
        PlaceBlock,
        Trade,
    }

    /// <summary>
    /// Triggered when the player moves.
    /// </summary>
    event EventHandler<PlayerMoveEventArgs> OnMove;

    /// <summary>
    /// The count of emeralds the player has.
    /// </summary>
    public int EmeraldCount { get; }
    /// <summary>
    /// Whether the player has a bed.
    /// </summary>
    public bool HasBed { get; }
    /// <summary>
    /// The spawn point of the player.
    /// </summary>
    public IPosition<float> SpawnPoint { get; }
    /// <summary>
    /// The count of wool blocks the player has.
    /// </summary>
    public int WoolCount { get; }

    /// <summary>
    /// Performs an action.
    /// </summary>
    /// <param name="actionKind">The action kind.</param>
    public void PerformAction(ActionKindType actionKind);
    /// <summary>
    /// Trades a commodity.
    /// </summary>
    /// <param name="commodityKind">The commodity kind.</param>
    public void Trade(CommodityKindType commodityKind);
}
