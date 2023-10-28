using EdcHost.ViewerServers.EventArgs;
using ErrorMessage = EdcHost.ViewerServers.Messages.Error;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleSetPortEvent(object? sender, SetPortEventArgs e)
    {
        try
        {
            if (_playerIdToPortName.Any(kvp => kvp.Value == e.PortName))
            {
                _logger.Error($"Port name {e.PortName} is taken by a player.");
                return;
            }

            if (_playerIdToPortName.ContainsKey(e.PlayerId) == false)
            {
                _slaveServer.OpenPort(
                    portName: e.PortName
                );
                _playerIdToPortName.Add(e.PlayerId, e.PortName);
            }
            else
            {
                string oldPortName = _playerIdToPortName[e.PlayerId];
                _slaveServer.OpenPort(
                    portName: e.PortName
                );
                _playerIdToPortName[e.PlayerId] = e.PortName;
                _slaveServer.ClosePort(oldPortName);
            }

            _logger.Information("[Update]");
            _logger.Information($"Player {e.PlayerId}:");
            _logger.Information($"Port: {e.PortName}");
        }
        catch (Exception exception)
        {
            _logger.Error($"Failed to set port: {exception}");
        }
    }

    void HandleSetCameraEvent(object? sender, SetCameraEventArgs e)
    {
        //TODO: Set camera


        _logger.Information("[Update]");
        _logger.Information($"Player {e.PlayerId}:");
        _logger.Information($"Camera: {e.CameraConfiguration}");
    }

    private void HandleStartGameEvent(object? sender, EventArgs e)
    {
        try
        {
            _gameRunner.Start();
        }
        catch (Exception exception)
        {
            string message = $"Failed to start game: {exception}";
            _logger.Error(message);

            // Send ERROR packet to the viewer
            _viewerServer.Publish(new ErrorMessage(messageType: "ERROR", errorCode: 0, message: message));
        }
    }

    private void HandleEndGameEvent(object? sender, EventArgs e)
    {
        try
        {
            _gameRunner.End();

        }
        catch (Exception exception)
        {
            string message = $"Failed to stop game: {exception}";
            _logger.Error(message);

            // Send ERROR packet to the viewer
            _viewerServer.Publish(new ErrorMessage(messageType: "ERROR", errorCode: 0, message: message));
        }
    }

    private void HandleResetGameEvent(object? sender, EventArgs e)
    {
        try
        {
            if (_gameRunner.IsRunning)
            {
                _gameRunner.End();
            }

            _game = Games.IGame.Create(
                diamondMines: _options.GameDiamondMines,
                goldMines: _options.GameGoldMines,
                ironMines: _options.GameIronMines
            );
            _gameRunner = Games.IGameRunner.Create(_game);

            _game.AfterGameStartEvent += HandleAfterGameStartEvent;
            _game.AfterGameTickEvent += HandleAfterGameTickEvent;
            _game.AfterJudgementEvent += HandleAfterJudgementEvent;

            for (int i = 0; i < _game.Players.Count; i++)
            {
                _game.Players[i].OnAttack += HandlePlayerAttackEvent;
                _game.Players[i].OnPlace += HandlePlayerPlaceEvent;
                _game.Players[i].OnDig += HandlePlayerDigEvent;
            }
        }
        catch (Exception exception)
        {
            string message = $"Failed to reset game: {exception}";
            _logger.Error(message);

            // Send ERROR packet to the viewer
            _viewerServer.Publish(new ErrorMessage(messageType: "ERROR", errorCode: 0, message: message));
        }
    }
}
