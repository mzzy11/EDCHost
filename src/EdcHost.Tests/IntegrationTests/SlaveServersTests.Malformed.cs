using System.Text;

using EdcHost.SlaveServers;

using Xunit;

namespace EdcHost.Tests.IntegrationTests;
public partial class SlaveServersTests
{
    [Theory]
    [InlineData(new byte[] { 0x55, 0xAA, 0x01, 0x02, 0x00, 0x01, 0x01 }, "COM1", 9600)] //length
    [InlineData(new byte[] { 0x55, 0xAA, 0x00, 0x03, 0x00, 0x01, 0x01 }, "COM1", 9600)] //length
    [InlineData(new byte[] { 0x55, 0xAA, 0x00, 0x02, 0x01, 0x01 }, "COM1", 9600)] //length
    [InlineData(new byte[] { 0x55, 0xAA, 0x00, 0x02, 0x01, 0x01, 0x01 }, "COM1", 9600)] //checksum
    [InlineData(new byte[] { 0x55, 0x00, 0x02, 0x00, 0x01, 0x01 }, "COM1", 9600)] //head
    [InlineData(new byte[] { 0xAA, 0x00, 0x02, 0x00, 0x01, 0x01 }, "COM1", 9600)] //head
    [InlineData(new byte[] { 0x53, 0xAA, 0x00, 0x02, 0x00, 0x01, 0x01 }, "COM1", 9600)] //head
    [InlineData(new byte[] { 0x55, 0xAB, 0x00, 0x02, 0x00, 0x01, 0x01 }, "COM1", 9600)] //head

    void Malformed(byte[] bytes, string portName, int baudRate)
    {
        // Arrange
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, new SerialPortWrapperMock(portName) }
            }
        };
        var slaveServer = new SlaveServer(serialPortHubMock);
        slaveServer.Start();

        // Act
        var serialPortWrapperMock = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);
        serialPortWrapperMock.MockReceive(bytes);

        // Assertion
        Assert.Throws<Exception>(() => slaveServer.Stop());
    }
}
