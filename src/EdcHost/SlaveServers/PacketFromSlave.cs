namespace EdcHost.SlaveServers;

public class PacketFromSlave : IPacketFromSlave
{
    public int ActionType { get; private set; }
    public int Param { get; private set; }

    public PacketFromSlave(byte[] bytes)
    {
        int currentIndex = 0;
        ActionType = Convert.ToInt32(bytes[currentIndex++]);
        Param = Convert.ToInt32(bytes[currentIndex]);
    }

    public byte[] ToBytes()
    {
        int datalength = 2;
        byte[] data = new byte[datalength];

        int currentIndex = 0;
        data[currentIndex++] = Convert.ToByte(ActionType);
        data[currentIndex] = Convert.ToByte(Param);

        //add header
        byte[] header = IPacket.GeneratePacketHeader(data);
        byte[] bytes = new byte[header.Length + data.Length];
        header.CopyTo(bytes, 0);
        data.CopyTo(bytes, header.Length);
        return bytes;
    }
}
