using EdcHost.SlaveServers;

namespace EdcHost.Tests.IntegrationTests;
class SerialPortHubMock : ISerialPortHub
{
    public Dictionary<string, SerialPortWrapperMock> SerialPorts = new();

    public ISerialPortWrapper Get(string portName)
    {
        if (!SerialPorts.ContainsKey(portName))
        {
            SerialPorts.Add(portName, new SerialPortWrapperMock());
        }

        return SerialPorts[portName];
    }
}
