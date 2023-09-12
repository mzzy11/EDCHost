namespace EdcHost.Games;

public class Mine : IMine
{
    #region Public properties

    public int GeneratedOreCount { get; private set; }
    public int OreKind { get; }
    public IPosition<float> Position { get; }

    #endregion

    #region Public methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="oreKind">Kind of the ore</param>
    /// <param name="position">Position of the mine</param>
    public Mine(int oreKind, Position<float> position)
    {
        GeneratedOreCount = 0;
        OreKind = oreKind;
        Position = position;
    }

    /// <summary>
    /// Generate a new ore.
    /// </summary>
    public void GenerateOre()
    {
        GeneratedOreCount++;
    }

    #endregion
}
