using System.Text;

using EdcHost.SlaveServers;

using Xunit;

namespace EdcHost.Tests.IntegrationTests;
public partial class SlaveServersTests
{
    [Theory]
    [InlineData(new byte[] { 0x01, 0x01 }, "COM1", 9600)]
    public void Reception(byte[] packetData, string portName, int baudRate)
    {
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, new SerialPortWrapperMock(portName) }
            }
        };
        var slaveServer = new SlaveServer(serialPortHubMock);
        slaveServer.Start();

        byte[] bytes = MakePacket(packetData);

        PacketFromSlave packet = new PacketFromSlave(bytes);

        var serialPortWrapperMock = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);
        serialPortWrapperMock.AfterReceive += (sender, args) =>
        {
            // Assertion
            Assert.Equal(portName, args.PortName);
            Assert.Equal(0x55, args.Bytes[0]);
            Assert.Equal(0xAA, args.Bytes[1]);
            Assert.Equal(0x00, args.Bytes[2]);
            Assert.Equal(0x02, args.Bytes[3]);
            Assert.Equal(0x00, args.Bytes[4]);
            Assert.Equal(0x01, args.Bytes[5]);
            Assert.Equal(0x01, args.Bytes[6]);
            Assert.Equal(new PacketFromSlave(packetData), packet);
            slaveServer.Stop();
        };
        serialPortWrapperMock.MockReceive(bytes);
    }
}
