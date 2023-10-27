namespace EdcHost.Games;

public class PlayerPickUpEventArgs : EventArgs
{
    /// <summary>
    /// The type of the mine that is picked up.
    /// </summary>
    public IMine.OreKindType MineType { get; }
    public int ItemCount { get; }

    public PlayerPickUpEventArgs(IMine.OreKindType mineType, int itemCount)
    {
        MineType = mineType;
        ItemCount = itemCount;
    }
}
