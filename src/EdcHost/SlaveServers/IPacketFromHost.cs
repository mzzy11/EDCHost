namespace EdcHost.SlaveServers;

public interface IPacketFromHost : IPacket
{
    static IPacketFromHost Create(
        int gameStage, int elapsedTime, List<int> heightOfChunks, bool hasBed, bool hasBedOpponent,
        float positionX, float positionY, float positionOpponentX, float positionOpponentY,
        int agility, int health, int maxHealth, int strength,
        int emeraldCount, int woolCount)
    {
        return new PacketFromHost(
            gameStage, elapsedTime, heightOfChunks, hasBed, hasBedOpponent,
            positionX, positionY, positionOpponentX, positionOpponentY,
            agility, health, maxHealth, strength,
            emeraldCount, woolCount
            );
    }

    public int GameStage { get; }
    public int ElapsedTime { get; }
    public List<int> HeightOfChunks { get; }
    public bool HasBed { get; }
    public float PositionX { get; }
    public float PositionY { get; }
    public float PositionOpponentX { get; }
    public float PositionOpponentY { get; }
    public int Agility { get; }
    public int Health { get; }
    public int MaxHealth { get; }
    public int Strength { get; }
    public int EmeraldCount { get; }
    public int WoolCount { get; }
}
