using System.Diagnostics.Contracts;

namespace EdcHost.SlaveServers;

public interface IPacket
{

    /// <summary>
    /// Extract the data from a packet in raw byte array form.
    /// </summary>
    /// <param name="bytes">
    /// The raw data.
    /// </param>
    /// <returns></returns>
    public byte[] ExtractPacketData(byte[] bytes);

    public void MakePacket(byte[] bytes);
}
