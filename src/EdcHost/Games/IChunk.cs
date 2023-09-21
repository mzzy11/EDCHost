namespace EdcHost.Games;

public interface IChunk
{
    /// <summary>
    /// Whether a block can be placed in the chunk.
    /// </summary>
    public bool CanPlaceBlock { get; }
    /// <summary>
    /// Whether a block can be removed from the chunk.
    /// </summary>
    public bool CanRemoveBlock { get; }
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

    /// <summary>
    /// Places a block in the chunk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the chunk cannot have a block placed in it.
    /// </exception>
    public void PlaceBlock();
    /// <summary>
    /// Removes a block from the chunk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the chunk cannot have a block removed from it.
    /// </exception>
    public void RemoveBlock();
}
