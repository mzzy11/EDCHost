namespace EdcHost.Games;

public class Position<T> : IPosition<T>
{
    #region Public properties

    public T X { get; set; }
    public T Y { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    public Position(T x, T y)
    {
        X = x;
        Y = y;
    }

    #endregion
}
