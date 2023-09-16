namespace EdcHost.Games;

public class Chunk : IChunk
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
    public bool IsVoid { get => (Height == 0); }

    /// <summary>
    /// The position of the chunk.
    /// </summary>
    public IPosition<int> Position { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="height">Height of the chunk</param>
    /// <param name="position">Position of the chunk</param>
    public Chunk(int height, IPosition<int> position)
    {
        Height = height;
        CanPlaceBlock = true;
        Position = position;
    }
}
