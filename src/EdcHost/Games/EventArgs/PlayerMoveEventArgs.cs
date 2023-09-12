namespace EdcHost.Games;

public class PlayerMoveEventArgs : EventArgs
{
    public IPlayer Player { get; }
    public IPosition<float> PositionBeforeMovement { get; }
    public IPosition<float> Position { get; }

    public PlayerMoveEventArgs(IPlayer player, IPosition<float> positionBeforeMovement, IPosition<float> position)
    {
        Player = player;
        PositionBeforeMovement = positionBeforeMovement;
        Position = position;
    }
}
