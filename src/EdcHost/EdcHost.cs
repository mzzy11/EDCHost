using System.Collections.Concurrent;
using Serilog;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    const int MapHeight = 8;
    const int MapWidth = 8;

    readonly ILogger _logger = Log.ForContext("Component", "EdcHost");
    readonly Dictionary<int, string> _playerIdToPortName = new();
    readonly Dictionary<int, int> _playerIdToCameraIndex = new();
    readonly CameraServers.ICameraServer _cameraServer;
    readonly SlaveServers.ISlaveServer _slaveServer;
    readonly ViewerServers.IViewerServer _viewerServer;
    readonly ConcurrentQueue<EventArgs> _playerEventQueue = new();
    readonly EdcHostOptions _options;

    Games.IGame _game;
    Games.IGameRunner _gameRunner;

    public EdcHost(EdcHostOptions options)
    {
        _options = options;

        _game = Games.IGame.Create(
            diamondMines: _options.GameDiamondMines,
            goldMines: _options.GameGoldMines,
            ironMines: _options.GameIronMines
        );
        _gameRunner = Games.IGameRunner.Create(_game);
        _cameraServer = CameraServers.ICameraServer.Create();
        _slaveServer = SlaveServers.ISlaveServer.Create();
        _viewerServer = ViewerServers.IViewerServer.Create(_options.ServerPort);

        _game.AfterGameStartEvent += HandleAfterGameStartEvent;
        _game.AfterGameTickEvent += HandleAfterGameTickEvent;
        _game.AfterJudgementEvent += HandleAfterJudgementEvent;

        for (int i = 0; i < _game.Players.Count; i++)
        {
            _game.Players[i].OnAttack += HandlePlayerAttackEvent;
            _game.Players[i].OnPlace += HandlePlayerPlaceEvent;
            _game.Players[i].OnDig += HandlePlayerDigEvent;
        }

        _slaveServer.PlayerTryAttackEvent += HandlePlayerTryAttackEvent;
        _slaveServer.PlayerTryTradeEvent += HandlePlayerTryTradeEvent;
        _slaveServer.PlayerTryPlaceBlockEvent += HandlePlayerTryPlaceBlockEvent;

        _viewerServer.SetCameraEvent += HandleSetCameraEvent;
        _viewerServer.SetPortEvent += HandleSetPortEvent;
        _viewerServer.Controller.StartGameEvent += HandleStartGameEvent;
        _viewerServer.Controller.EndGameEvent += HandleEndGameEvent;
        _viewerServer.Controller.ResetGameEvent += HandleResetGameEvent;
    }

    public void Start()
    {
        _logger.Information("Starting...");

        try
        {
            _cameraServer.Start();
            while (_cameraServer.AvailableCameraIndexes.Count == 0)
            {
            }
            _cameraServer.OpenCamera(_cameraServer.AvailableCameraIndexes[0]);
        }
        catch (Exception e)
        {
            _logger.Error($"failed to start camera server: {e}");
        }

        try
        {
            _slaveServer.Start();
            // _slaveServer.OpenPort("COM1");
            // _playerIdToPortName.Add(0, "COM1");
        }
        catch (Exception e)
        {
            _logger.Error($"failed to start slave server: {e}");
        }

        try
        {
            _viewerServer.Start();
        }
        catch (Exception e)
        {
            _logger.Error($"failed to start viewer server: {e}");
        }

        _logger.Information("Started.");
    }

    public void Stop()
    {
        _logger.Information("Stopping...");

        try
        {
            _cameraServer.Stop();
        }
        catch (Exception e)
        {
            _logger.Error($"failed to stop camera server: {e}");
        }

        try
        {
            _slaveServer.Stop();
        }
        catch (Exception e)
        {
            _logger.Error($"failed to stop slave server: {e}");
        }

        try
        {
            _viewerServer.Stop();
        }
        catch (Exception e)
        {
            _logger.Error($"failed to stop viewer server: {e}");
        }

        _logger.Information("Stopped.");
    }
}
