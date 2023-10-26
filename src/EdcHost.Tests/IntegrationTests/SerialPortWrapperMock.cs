using EdcHost.SlaveServers;

namespace EdcHost.Tests.IntegrationTests;

class SerialPortWrapperMock : ISerialPortWrapper
{
    public bool IsOpen = false;
    public List<byte> ReadBuffer = new();
    public List<byte> WriteBuffer = new();

    public int BytesToRead => ReadBuffer.Count;
    public void Close()
    {
        IsOpen = false;
    }
    public void Open()
    {
        if (IsOpen)
        {
            throw new InvalidOperationException();
        }

        IsOpen = true;
    }
    public void Read(byte[] buffer, int offset, int count)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException();
        }

        if (count > buffer.Length - offset)
        {
            throw new ArgumentException();
        }

        Array.Copy(ReadBuffer.ToArray(), 0, buffer, offset, count);
        ReadBuffer.Clear();
    }
    public void Write(byte[] buffer, int offset, int count)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException();
        }

        if (count > buffer.Length - offset)
        {
            throw new ArgumentException();
        }

        WriteBuffer.Clear();
        WriteBuffer.AddRange(buffer.Skip(offset).Take(count));
    }
}
