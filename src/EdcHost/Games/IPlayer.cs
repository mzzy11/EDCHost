namespace EdcHost.Games;

public interface IPlayer
{
    event EventHandler<PlayerMoveEventArgs> OnMove;

    public enum CommodityKindType
    {
        AgilityBoost,
        HealthBoost,
        HealthPotion,
        StrengthBoost,
        Wool,
    }

    public enum ActionKindType
    {
        Attack,
        PlaceBlock,
        Trade,
    }

    public int EmeraldCount { get; }
    public bool HasBed { get; }
    public IPosition<float> SpawnPoint { get; }

    public bool PerformAction(ActionKindType actionKind);
    public bool Trade(CommodityKindType commodityKind);
}
