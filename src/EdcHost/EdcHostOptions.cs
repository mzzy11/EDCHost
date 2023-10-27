namespace EdcHost;

public record EdcHostOptions
{
    public List<Tuple<int, int>> GameDiamondMines { get; init; } = new();
    public List<Tuple<int, int>> GameGoldMines { get; init; } = new();
    public List<Tuple<int, int>> GameIronMines { get; init; } = new();
    public int ServerPort { get; init; }
}
