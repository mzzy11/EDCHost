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
    public void Noise(int randomSeed)
    {
        const string PortName = "COM1";
        const int MaxLength = 100000000;
        const int SlaveToHostBytesCount = 7;
        const int SlaveToHostDataBytesCount = 2;

        // Arrange
        SerialPortWrapperMock serialPortWrapperMock = new(PortName);
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { PortName, serialPortWrapperMock }
            }
        };
        Random random = new(randomSeed);
        ISlaveServer slaveServer = new SlaveServer(serialPortHubMock);

        // Act
        slaveServer.Start();
        slaveServer.OpenPort(PortName, 0);

        Assert.True(serialPortWrapperMock.IsOpen);

        // Act: random bytes
        serialPortWrapperMock.MockReceive(GenerateRandomBytes(random, random.Next(0, MaxLength)));

        // Act: random body
        serialPortWrapperMock.MockReceive(MakePacket(GenerateRandomBytes(random, random.Next(0, MaxLength))));

        // Act: random value
        byte[] readBufferBytes = new byte[SlaveToHostBytesCount];
        readBufferBytes[5] = (byte)random.Next(0, 256);
        readBufferBytes[6] = (byte)random.Next(0, 256);
        // Headers should be generated after data bytes are set.
        readBufferBytes[0] = (byte)0x55;
        readBufferBytes[1] = (byte)0xAA;
        BitConverter.GetBytes((short)SlaveToHostDataBytesCount).CopyTo(readBufferBytes, 2);
        readBufferBytes[4] = CalculateChecksum(readBufferBytes.ToArray()[5..SlaveToHostBytesCount]);
        serialPortWrapperMock.MockReceive(readBufferBytes);
    }
}
