using EdcHost.ViewerServers.EventArgs;

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
            _logger.Error($"Failed to start game: {exception}");
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
            _logger.Error($"Failed to stop game: {exception}");
        }
    }

    private void HandleResetGameEvent(object? sender, EventArgs e)
    {
        try
        {
            //TODO: Reset game
        }
        catch (Exception exception)
        {
            _logger.Error($"Failed to reset game: {exception}");
        }
    }
}
