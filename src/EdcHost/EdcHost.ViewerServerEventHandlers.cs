using System.Reflection.Metadata;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleAfterMessageReceiveEvent(object? sender, ViewerServers.AfterMessageReceiveEventArgs e)
    {
        switch (e.Message)
        {
            case ViewerServers.CompetitionControlCommandMessage message:
                switch (message.Command)
                {
                    case "START":
                        HandleStartGame();
                        break;

                    case "END":
                        HandleEndGame();
                        break;

                    case "RESET":
                        HandleResetGame();
                        break;

                    default:
                        _logger.Error($"Invalid command: {message.Command}.");

#if DEBUG
                        throw new Exception($"invalid command: {message.Command}");
#else
                        break;
#endif
                }
                break;

            case ViewerServers.HostConfigurationFromClientMessage message:
                HandleUpdateConfiguration(message);
                break;

            default:
                _logger.Error($"Invalid message type: {e.Message.MessageType}.");

#if DEBUG
                throw new Exception($"invalid message type: {e.Message.MessageType}");
#else
                break;
#endif
        }
    }

    void HandleStartGame()
    {
        _gameRunner.Start();
    }

    void HandleEndGame()
    {
        _gameRunner.End();
    }

    void HandleResetGame()
    {
        if (_gameRunner.IsRunning)
        {
            _gameRunner.End();
        }

        _game = Games.IGame.Create(
            diamondMines: _config.Game.DiamondMines,
            goldMines: _config.Game.GoldMines,
            ironMines: _config.Game.IronMines
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

    void HandleUpdateConfiguration(ViewerServers.HostConfigurationFromClientMessage message)
    {
        foreach (ViewerServers.HostConfigurationFromClientMessage.PlayerType player in message.Players)
        {
            // Do not need to check if a player exists because we do not care.

            PlayerHardwareInfo playerHardwareInfo = new();

            if (player.Camera is not null)
            {
                playerHardwareInfo.CameraIndex = player.Camera.CameraId;

                CameraServers.ICamera camera = _cameraServer.GetCamera(player.Camera.CameraId)
                    ?? _cameraServer.OpenCamera(player.Camera.CameraId, new CameraServers.Locator());

                CameraServers.RecognitionOptions recognitionOptions = new()
                {
                    HueCenter = player.Camera.Recognition.HueCenter,
                    HueRange = player.Camera.Recognition.HueRange,
                    SaturationCenter = player.Camera.Recognition.SaturationCenter,
                    SaturationRange = player.Camera.Recognition.SaturationRange,
                    ValueCenter = player.Camera.Recognition.ValueCenter,
                    ValueRange = player.Camera.Recognition.ValueRange,
                    MinArea = player.Camera.Recognition.MinArea,
                    ShowMask = player.Camera.Recognition.ShowMask
                };

                if (player.Camera.Calibration is not null)
                {
                    recognitionOptions.Calibrate = true;

                    recognitionOptions.TopLeftX = player.Camera.Calibration.TopLeft.X;
                    recognitionOptions.TopLeftY = player.Camera.Calibration.TopLeft.Y;
                    recognitionOptions.TopRightX = player.Camera.Calibration.TopRight.X;
                    recognitionOptions.TopRightY = player.Camera.Calibration.TopRight.Y;
                    recognitionOptions.BottomLeftX = player.Camera.Calibration.BottomLeft.X;
                    recognitionOptions.BottomLeftY = player.Camera.Calibration.BottomLeft.Y;
                    recognitionOptions.BottomRightX = player.Camera.Calibration.BottomRight.X;
                    recognitionOptions.BottomRightY = player.Camera.Calibration.BottomRight.Y;
                }

                camera.Locator = new CameraServers.Locator(recognitionOptions);
            }

            if (player.SerialPort is not null)
            {
                playerHardwareInfo.PortName = player.SerialPort.PortName;

                _slaveServer.OpenPort(
                    portName: player.SerialPort.PortName,
                    baudRate: player.SerialPort.BaudRate
                );
            }

            _playerHardwareInfo.AddOrUpdate(player.PlayerId, playerHardwareInfo, (_, _) => playerHardwareInfo);
        }
    }
}
