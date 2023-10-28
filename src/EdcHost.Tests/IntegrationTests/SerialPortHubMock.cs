using EdcHost.SlaveServers;

namespace EdcHost.Tests.IntegrationTests;
class SerialPortHubMock : ISerialPortHub
{
    public Dictionary<string, SerialPortWrapperMock> SerialPorts = new();

    public List<string> PortNames => SerialPorts.Keys.ToList();

    public ISerialPortWrapper Get(string portName, int baudRate)
    {
        if (!SerialPorts.ContainsKey(portName))
        {
            throw new ArgumentException($"port name does not exist: {portName}");
        }

        return SerialPorts[portName];
    }
}
