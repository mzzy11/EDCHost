using System.Collections.Concurrent;
using EdcHost.SlaveServers.EventArgs;
using Serilog;

namespace EdcHost.SlaveServers;

public class SlaveServer : ISlaveServer
{
    record PortComponentBundle
    {
        public ConcurrentQueue<IPacketFromHost> PacketsToSend;
        public ConcurrentQueue<IPacketFromSlave> PacketsReceived;
        public ISerialPortWrapper SerialPort;
        public bool ShouldRun = true;
        public Task TaskForSending;
        public Task TaskForReceiving;

        public PortComponentBundle(ISerialPortWrapper serialPort, Task taskForSending,
            Task taskForReceiving, ConcurrentQueue<IPacketFromHost> packetsToSend,
            ConcurrentQueue<IPacketFromSlave> packetsReceived)
        {
            SerialPort = serialPort;
            TaskForSending = taskForSending;
            TaskForReceiving = taskForReceiving;
            PacketsToSend = packetsToSend;
            PacketsReceived = packetsReceived;
        }
    }

    public event EventHandler<PlayerTryAttackEventArgs>? PlayerTryAttackEvent;
    public event EventHandler<PlayerTryPlaceBlockEventArgs>? PlayerTryPlaceBlockEvent;
    public event EventHandler<PlayerTryTradeEventArgs>? PlayerTryTradeEvent;

    readonly ILogger _logger = Log.Logger.ForContext("Component", "SlaveServers");
    readonly ConcurrentDictionary<string, PortComponentBundle> _portComponentBundles = new();
    readonly ISerialPortHub _serialPortHub;

    public SlaveServer(ISerialPortHub serialPortHub)
    {
        _serialPortHub = serialPortHub;
    }

    public void OpenPort(string portName)
    {
        if (_portComponentBundles.Keys.Any(portName.Equals))
        {
            throw new ArgumentException($"port name already exists: {portName}");
        }

        ISerialPortWrapper serialPort = _serialPortHub.Get(portName);
        serialPort.Open();

        ConcurrentQueue<IPacketFromHost> packetsToSend = new();
        ConcurrentQueue<IPacketFromSlave> packetsReceived = new();
        Task taskForSending = new(() => SendTaskFunc(portName));
        Task taskForReceiving = new(() => ReceiveTaskFunc(portName));

        var portComponentBundle = new PortComponentBundle(serialPort, taskForSending,
            taskForReceiving, packetsToSend, packetsReceived);

        _portComponentBundles.TryAdd(portName, portComponentBundle);

        taskForSending.Start();
        taskForReceiving.Start();
    }

    public void ClosePort(string portName)
    {
        if (!_portComponentBundles.Keys.Any(portName.Equals))
        {
            throw new ArgumentException($"port name does not exist: {portName}");
        }

        _portComponentBundles[portName].ShouldRun = false;
        _portComponentBundles[portName].TaskForSending.Wait();
        _portComponentBundles[portName].TaskForReceiving.Wait();
        _portComponentBundles[portName].SerialPort.Close();
        _portComponentBundles.Remove(portName, out _);
    }

    public void Publish(string portName, int gameStage, int elapsedTime, List<int> heightOfChunks,
        bool hasBed, bool hasBedOpponent, double positionX, double positionY, double positionOpponentX,
        double positionOpponentY, int agility, int health, int maxHealth, int strength,
        int emeraldCount, int woolCount)
    {
        if (!_portComponentBundles.Keys.Any(portName.Equals))
        {
            throw new ArgumentException($"port name does not exist: {portName}");
        }

        _portComponentBundles[portName].PacketsToSend.Enqueue(new PacketFromHost(gameStage, elapsedTime, heightOfChunks,
            hasBed, hasBedOpponent, (float)positionX, (float)positionY, (float)positionOpponentX, (float)positionOpponentY, agility, health, maxHealth, strength,
            emeraldCount, woolCount));
    }

    public void Start()
    {
        _logger.Information("Starting...");

        // Do nothing.

        _logger.Information("Started.");
    }

    public void Stop()
    {
        _logger.Information("Stopping...");

        foreach (PortComponentBundle portComponentBundle in _portComponentBundles.Values)
        {
            portComponentBundle.ShouldRun = false;
            portComponentBundle.TaskForSending.Wait();
            portComponentBundle.TaskForReceiving.Wait();
            portComponentBundle.SerialPort.Close();
        }

        _portComponentBundles.Clear();

        _logger.Information("Stopped.");
    }

    void PerformAction(string portName, IPacketFromSlave packet)
    {
        switch (packet.ActionType)
        {
            case (int)ActionKind.Attack:
                PlayerTryAttackEvent?.Invoke(this, new PlayerTryAttackEventArgs(portName, packet.Param));
                break;

            case (int)ActionKind.Use:
                PlayerTryPlaceBlockEvent?.Invoke(this, new PlayerTryPlaceBlockEventArgs(portName, packet.Param));
                break;

            case (int)ActionKind.Trade:
                PlayerTryTradeEvent?.Invoke(this, new PlayerTryTradeEventArgs(portName, packet.Param));
                break;

            default:
                break;
        }
    }

    void SendTaskFunc(string portName)
    {
        _logger.Debug("SendTaskFunc started");

        while (_portComponentBundles.GetValueOrDefault(portName)?.ShouldRun ?? false)
        {
            try
            {
                if (_portComponentBundles[portName].PacketsToSend.TryDequeue(out IPacketFromHost? packet))
                {
                    byte[] message = packet.ToBytes();
                    _portComponentBundles[portName].SerialPort.Write(message, 0, message.Length);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "error while sending packet");
            }
        }

        _logger.Debug("SendTaskFunc stopped");
    }

    void ReceiveTaskFunc(string portName)
    {
        _logger.Debug($"ReceiveTaskFunc of {portName} started");

        while (_portComponentBundles.GetValueOrDefault(portName)?.ShouldRun ?? false)
        {
            try
            {
                if (_portComponentBundles[portName].SerialPort.BytesToRead == 0)
                {
                    continue;
                }
                byte[] message = new byte[_portComponentBundles[portName].SerialPort.BytesToRead];
                _portComponentBundles[portName].SerialPort.Read(message, 0, message.Length);
                _portComponentBundles[portName].PacketsReceived.Enqueue(new PacketFromSlave(message));

                while (_portComponentBundles[portName].PacketsReceived.TryDequeue(out IPacketFromSlave? packet))
                {
                    PerformAction(portName, packet);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "error while receiving packet");
            }
        }

        _logger.Debug("ReceiveTaskFunc of {portName} stopped");
    }
}
