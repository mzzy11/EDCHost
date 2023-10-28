namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleAfterGameStartEvent(object? sender, Games.AfterGameStartEventArgs e)
    {
        //Do nothing
    }

    void HandleAfterGameTickEvent(object? sender, Games.AfterGameTickEventArgs e)
    {
        try
        {
            List<int> heightOfChunks = new();
            foreach (Games.IChunk chunk in e.Game.GameMap.Chunks)
            {
                heightOfChunks.Add(chunk.Height);
            }

            for (int i = 0; i < 2; i++)
            {
                string? portName = _playerHardwareInfo.GetValueOrDefault(e.Game.Players[i].PlayerId).PortName;
                if (portName is null)
                {
                    continue;
                }

                _slaveServer.Publish(
                    portName: portName,
                    gameStage: (int)e.Game.CurrentStage,
                    elapsedTime: e.Game.ElapsedTicks,
                    heightOfChunks: heightOfChunks,
                    hasBed: e.Game.Players[i].HasBed,
                    hasBedOpponent: e.Game.Players.Any(player => player.HasBed && player.PlayerId != e.Game.Players[i].PlayerId),
                    positionX: e.Game.Players[i].PlayerPosition.X,
                    positionY: e.Game.Players[i].PlayerPosition.Y,
                    positionOpponentX: e.Game.Players[(i == 0) ? 1 : 0].PlayerPosition.X,
                    positionOpponentY: e.Game.Players[(i == 0) ? 1 : 0].PlayerPosition.Y,
                    agility: e.Game.Players[i].ActionPoints,
                    health: e.Game.Players[i].Health,
                    maxHealth: e.Game.Players[i].MaxHealth,
                    strength: e.Game.Players[i].Strength,
                    emeraldCount: e.Game.Players[i].EmeraldCount,
                    woolCount: e.Game.Players[i].WoolCount
                );
            }

            // // Black image for test
            // int defaultImageWidth = 640;
            // int defaultImageHeight = 480;

            // // Fill it in black
            // using Mat demoImage = new(defaultImageHeight, defaultImageWidth, Emgu.CV.CvEnum.DepthType.Cv8U, 3);

            // byte[] jpegData = demoImage.ToImage<Bgr, byte>().ToJpegData();
            // string base64Image = Convert.ToBase64String(jpegData);

            List<ViewerServers.CompetitionUpdateMessage.Camera> cameraInfoList = new();
            foreach (int cameraIndex in _cameraServer.AvailableCameraIndexes)
            {
                CameraServers.ICamera? camera = _cameraServer.GetCamera(cameraIndex);
                if (camera?.IsOpened ?? false)
                {
                    cameraInfoList.Add(new ViewerServers.CompetitionUpdateMessage.Camera
                    {
                        cameraId = cameraIndex,
                        height = camera.Height,
                        width = camera.Width,
                        frameData = Convert.ToBase64String(camera.JpegData ?? new byte[] { })
                    });
                }
            }


            // Events for this tick;
            List<ViewerServers.CompetitionUpdateMessage.Event> currentEvents = new();
            while (!_playerEventQueue.IsEmpty)
            {
                if (_playerEventQueue.TryDequeue(out EventArgs? playerEvent) && playerEvent is not null)
                {
                    ViewerServers.CompetitionUpdateMessage.Event currentEvent = new();
                    switch (playerEvent)
                    {
                        case Games.PlayerDigEventArgs digEvent:
                            currentEvent = new()
                            {
                                playerDigEvent = new()
                                {
                                    playerId = digEvent.Player.PlayerId,
                                    targetChunk = digEvent.TargetChunk
                                }
                            };
                            break;
                        case Games.PlayerPickUpEventArgs pickUpEvent:
                            currentEvent = new()
                            {
                                playerPickUpEvent = new()
                                {
                                    playerId = pickUpEvent.Player.PlayerId,
                                    itemCount = pickUpEvent.ItemCount,
                                    itemType = (ViewerServers.CompetitionUpdateMessage.Event.PlayerPickUpEvent.ItemType)pickUpEvent.MineType
                                }
                            };
                            break;
                        case Games.PlayerPlaceEventArgs placeEvent:
                            currentEvent = new()
                            {
                                playerPlaceBlockEvent = new()
                                {
                                    playerId = placeEvent.Player.PlayerId
                                    // TODO: finish the event param
                                }
                            };
                            break;
                        // TODO: finish other cases
                        default:
                            break;
                    }
                    currentEvents.Add(currentEvent);
                }
            }


            // Send packet to the viewer
            _viewerServer.Publish(new ViewerServers.CompetitionUpdateMessage()
            {
                cameras = cameraInfoList,

                chunks = e.Game.GameMap.Chunks.Select(chunk => new ViewerServers.CompetitionUpdateMessage.Chunk()
                {
                    chunkId = chunk.Position != null ? chunk.Position.X + chunk.Position.Y * 8 : -1,
                    height = chunk.Height,
                    position = chunk.Position != null ? new ViewerServers.CompetitionUpdateMessage.Chunk.Position()
                    {
                        x = chunk.Position.X,
                        y = chunk.Position.Y
                    } : null
                }).ToList(),

                events = currentEvents,

                info = new ViewerServers.CompetitionUpdateMessage.Info()
                {
                    stage = e.Game.CurrentStage switch
                    {
                        Games.IGame.Stage.Ready => ViewerServers.CompetitionUpdateMessage.Info.Stage.Ready,
                        Games.IGame.Stage.Running => ViewerServers.CompetitionUpdateMessage.Info.Stage.Running,
                        Games.IGame.Stage.Ended => ViewerServers.CompetitionUpdateMessage.Info.Stage.Ended,
                        Games.IGame.Stage.Finished => ViewerServers.CompetitionUpdateMessage.Info.Stage.Finished,
                        Games.IGame.Stage.Battling => ViewerServers.CompetitionUpdateMessage.Info.Stage.Battling,
                        _ => throw new NotImplementedException($"{e.Game.CurrentStage} is not implemented")
                    },
                    elapsedTicks = e.Game.ElapsedTicks
                },

                mines = e.Game.Mines.Select(mine => new ViewerServers.CompetitionUpdateMessage.Mine()
                {
                    mineId = mine.MineId.ToString(),
                    accumulatedOreCount = mine.AccumulatedOreCount,
                    oreType = (ViewerServers.CompetitionUpdateMessage.Mine.OreType)mine.OreKind,
                    position = new ViewerServers.CompetitionUpdateMessage.Mine.Position()
                    {
                        x = mine.Position.X,
                        y = mine.Position.Y
                    }
                }).ToList(),

                players = e.Game.Players.Select(player => new ViewerServers.CompetitionUpdateMessage.Player()
                {
                    playerId = (player.PlayerId),

                    // TODO: Find the correspondence between the camera and the player 
                    cameraId = player.PlayerId,

                    attributes = new()
                    {
                        agility = player.ActionPoints,
                        strength = player.Strength,
                        maxHealth = player.MaxHealth
                    },
                    health = player.Health,
                    homePosition = new ViewerServers.CompetitionUpdateMessage.Player.HomePosition()
                    {
                        x = player.SpawnPoint.X,
                        y = player.SpawnPoint.Y,
                    },
                    inventory = new ViewerServers.CompetitionUpdateMessage.Player.Inventory()
                    {
                        emerald = player.EmeraldCount,
                        wool = player.WoolCount
                    },
                    position = new ViewerServers.CompetitionUpdateMessage.Player.Position()
                    {
                        x = player.PlayerPosition.X,
                        y = player.PlayerPosition.Y
                    }
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.Error($"Error while sending data to viewer: {ex.Message}");
#if DEBUG
            throw;
#endif
        }
    }



    void HandleAfterJudgementEvent(object? sender, Games.AfterJudgementEventArgs e)
    {
        if (e.Winner is null)
        {
            _logger.Information("No winner.");
        }
        else
        {
            _logger.Information($"Winner is {e.Winner?.PlayerId}");
        }

        _gameRunner.End();
    }
    void HandlePlayerDigEvent(object? sender, Games.PlayerDigEventArgs e)
    {
        // Store the event info to the queue
        _playerEventQueue.Enqueue(e);
    }
    void HandlePlayerAttackEvent(object? sender, Games.PlayerAttackEventArgs e)
    {
        // Store the event info to the queue
        _playerEventQueue.Enqueue(e);
    }
    void HandlePlayerPlaceEvent(object? sender, Games.PlayerPlaceEventArgs e)
    {
        // Store the event info to the queue
        _playerEventQueue.Enqueue(e);
    }
}
