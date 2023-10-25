namespace EdcHost.SlaveServers;

public interface IPacketFromSlave : IPacket
{
    static IPacketFromSlave Create(byte[] bytes) {
        return new PacketFromSlave(bytes);
    }

    public int ActionType { get; }
    public int Param { get; }
}
