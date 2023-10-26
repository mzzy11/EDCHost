using EdcHost.SlaveServers.EventArgs;

namespace EdcHost.SlaveServers;

/// <summary>
/// SlaveServer handles the communication with the slaves via UART.
/// </summary>
public interface ISlaveServer
{
    static ISlaveServer Create()
    {
        ISerialPortHub serialPortHub = new SerialPortHub();

        return new SlaveServer(serialPortHub);
    }

    event EventHandler<PlayerTryAttackEventArgs>? PlayerTryAttackEvent;
    event EventHandler<PlayerTryPlaceBlockEventArgs>? PlayerTryPlaceBlockEvent;
    event EventHandler<PlayerTryTradeEventArgs>? PlayerTryTradeEvent;

    /// <summary>
    /// Starts the server.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();

    void OpenPort(string portName);

    void ClosePort(string portName);

    void Publish(string portName, int gameStage, int elapsedTime, List<int> heightOfChunks,
        bool hasBed, bool hasBedOpponent, float positionX, float positionY, float positionOpponentX,
        float positionOpponentY, int agility, int health, int maxHealth, int strength,
        int emeraldCount, int woolCount);
}
