namespace EdcHost.Games;

public class Mine : IMine
{
    #region Public properties

    public int AccumulatedOreCount { get; private set; }
    public IMine.OreKindType OreKind { get; }
    public IPosition<float> Position { get; }

    #endregion

    #region Public methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="oreKind">The kind of the ore</param>
    /// <param name="position">Position of the mine</param>
    public Mine(IMine.OreKindType oreKind, Position<float> position)
    {
        AccumulatedOreCount = 0;
        OreKind = oreKind;
        Position = position;
    }

    public void PickUpOre(int count)
    {
        AccumulatedOreCount = Math.Max(0, AccumulatedOreCount - count);
    }

    #endregion
}
