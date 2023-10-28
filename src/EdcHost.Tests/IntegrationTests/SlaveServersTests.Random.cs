using System.Text;
using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class SlaveServersTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(11)]
    [InlineData(114)]
    [InlineData(1145)]
    [InlineData(11451)]
    [InlineData(114514)]
    [InlineData(19)]
    [InlineData(191)]
    [InlineData(1919)]
    [InlineData(19198)]
    [InlineData(191981)]
    [InlineData(1919810)]
    public void Random(int randomSeed)
    {
        const int MaxLength = 100000000;
        const int SlaveToHostBytesCount = 7;
        const int SlaveToHostDataBytesCount = 2;

        // Arrange
        Random random = new(randomSeed);
        string portName = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength)));
        SerialPortWrapperMock serialPortWrapperMock = new(portName);
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, serialPortWrapperMock }
            }
        };
        ISlaveServer slaveServer = new SlaveServer(serialPortHubMock);

        // Act
        slaveServer.Start();
        slaveServer.OpenPort(portName, 0);

        Assert.True(serialPortWrapperMock.IsOpen);

        // Act: random bytes
        serialPortWrapperMock.MockReceive(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength)));

        // Act: random body
        serialPortWrapperMock.MockReceive(MakePacket(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))));

        // Act: random value
        byte[] readBufferBytes = new byte[SlaveToHostBytesCount];
        readBufferBytes[5] = (byte)random.Next(0, 255);
        readBufferBytes[6] = (byte)random.Next(0, 255);
        // Headers should be generated after data bytes are set.
        readBufferBytes[0] = 0x55;
        readBufferBytes[1] = 0xAA;
        BitConverter.GetBytes((short)SlaveToHostDataBytesCount).CopyTo(readBufferBytes, 2);
        readBufferBytes[4] = CalculateChecksum(readBufferBytes.ToArray()[5..SlaveToHostBytesCount]);
        serialPortWrapperMock.MockReceive(readBufferBytes);
    }
}
