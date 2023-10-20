using Serilog;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    readonly Games.IGame _game;
    readonly Games.IGameRunner _gameRunner;
    readonly SlaveServers.ISlaveServer _slaveServer;
    readonly ViewerServers.IViewerServer _viewerServer;

    public EdcHost(Games.IGame game, Games.IGameRunner gameRunner, SlaveServers.ISlaveServer slaveServer, ViewerServers.IViewerServer viewerServer)
    {
        _game = game;
        _gameRunner = gameRunner;
        _slaveServer = slaveServer;
        _viewerServer = viewerServer;

        _game.AfterGameStartEvent += HandleAfterGameStartEvent;
        _game.AfterGameTickEvent += HandleAfterGameTickEvent;
        _game.AfterJudgementEvent += HandleAfterJudgementEvent;

        _slaveServer.PlayerTryAttackEvent += HandlePlayerTryAttackEvent;
        _slaveServer.PlayerTryTradeEvent += HandlePlayerTryTradeEvent;
        _slaveServer.PlayerTryUseEvent += HandlePlayerTryUseEvent;

        _viewerServer.SetCameraEvent += HandleSetCameraEvent;
        _viewerServer.SetPortEvent += HandleSetPortEvent;
    }

    public void Start()
    {
        Log.Information("Starting...");

        try
        {
            _slaveServer.Start();
        }
        catch (Exception e)
        {
            Log.Error($"failed to start slave server: {e}");
        }

        try
        {
            _viewerServer.Start();
        }
        catch (Exception e)
        {
            Log.Error($"failed to start viewer server: {e}");
        }

        try
        {
            _gameRunner.Start();
        }
        catch (Exception e)
        {
            Log.Error($"failed to start game runner: {e}");
        }

        Log.Information("Started.");
    }

    public void Stop()
    {
        Log.Information("Stopping...");

        try
        {
            _slaveServer.Stop();
        }
        catch (Exception e)
        {
            Log.Error($"failed to stop slave server: {e}");
        }

        try
        {
            _viewerServer.Stop();
        }
        catch (Exception e)
        {
            Log.Error($"failed to stop viewer server: {e}");
        }

        try
        {
            _gameRunner.End();
        }
        catch (Exception e)
        {
            Log.Error($"failed to end game runner: {e}");
        }

        Log.Information("Stopped.");
    }
}
