namespace EdcHost.Games;

public interface IChunk
{
    /// <summary>
    /// Whether blocks can be placed in the chunk.
    /// </summary>
    public bool CanPlaceBlock { get; }
    /// <summary>
    /// The height of the chunk.
    /// </summary>
    public int Height { get; }
    /// <summary>
    /// Whether the chunk is void.
    /// </summary>
    public bool IsVoid { get; }
    /// <summary>
    /// The position of the chunk.
    /// </summary>
    public IPosition<int> Position { get; }
}
