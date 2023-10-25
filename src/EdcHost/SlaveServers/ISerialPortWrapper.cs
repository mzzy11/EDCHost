namespace EdcHost.SlaveServers;

public interface ISerialPortWrapper {
    static ISerialPortWrapper Create(string portName) {
        return new SerialPortWrapper(portName);
    }

    int BytesToRead { get; }
    void Close();
    void Open();
    void Read(byte[] buffer, int offset, int count);
    void Write(byte[] buffer, int offset, int count);
}
