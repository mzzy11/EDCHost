using System.Text;
using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class SlaveServersTests
{
    [Theory]
    [InlineData(100, "COM1", 115200)]
    public async Task Concurrency(int clientCount, string portName, int baudRate)
    {
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, new SerialPortWrapperMock(portName) }
            }
        };
        var slaveServer = new SlaveServer(serialPortHubMock);
        slaveServer.Start();
        //do concurrency test
        var tasks = new List<Task>();
        for (int i = 0; i < clientCount; i++)
        {
            tasks.Add(Task.Run(() => {
                var serialPortWrapperMock = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);
                serialPortWrapperMock.MockReceive(Encoding.UTF8.GetBytes("{}"));
            }));
        }
        await Task.WhenAll(tasks);
        var serialPortWrapperMock = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);
        //Assertion
        Assert.Equal(clientCount, serialPortWrapperMock.WriteBuffer.Count);
        
        slaveServer.Stop();
    }
}
