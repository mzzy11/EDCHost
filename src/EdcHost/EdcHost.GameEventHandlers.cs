using Emgu.CV;
using Emgu.CV.Structure;

using EdcHost.Games;

using CompetitionUpdate = EdcHost.ViewerServers.Messages.CompetitionUpdate;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleAfterGameStartEvent(object? sender, AfterGameStartEventArgs e)
    {
        //Do nothing
    }

    void HandleAfterGameTickEvent(object? sender, AfterGameTickEventArgs e)
    {
        try
        {
            List<int> heightOfChunks = new();
            foreach (IChunk chunk in e.Game.GameMap.Chunks)
            {
                heightOfChunks.Add(chunk.Height);
            }

            for (int i = 0; i < 2; i++)
            {
                string? portName = _playerIdToPortName.GetValueOrDefault(e.Game.Players[i].PlayerId);
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

            List<CompetitionUpdate.Camera> cameraInfoList = new();
            foreach (int cameraIndex in _cameraServer.AvailableCameraIndexes)
            {
                CameraServers.ICamera camera = _cameraServer.GetCamera(cameraIndex);
                if (camera.IsOpened && camera.JpegData is not null)
                {
                    cameraInfoList.Add(new CompetitionUpdate.Camera()
                    {
                        cameraId = cameraIndex,
                        height = camera.Height,
                        width = camera.Width,
                        frameData = Convert.ToBase64String(camera.JpegData)
                    });
                }
            }


            // Events for this tick;
            List<CompetitionUpdate.Event> currentEvents = new();
            while (!_playerEventQueue.IsEmpty)
            {
                if (_playerEventQueue.TryDequeue(out EventArgs? playerEvent) && playerEvent is not null)
                {
                    CompetitionUpdate.Event currentEvent = new CompetitionUpdate.Event();
                    switch (playerEvent)
                    {
                        case PlayerDigEventArgs digEvent:
                            currentEvent.playerDigEvent = new()
                            {
                                playerId = digEvent.Player.PlayerId,
                                targetChunk = digEvent.TargetChunk
                            };
                            break;
                        case PlayerPickUpEventArgs pickUpEvent:
                            currentEvent.playerPickUpEvent = new()
                            {
                                playerId = pickUpEvent.Player.PlayerId,
                                itemCount = pickUpEvent.ItemCount,
                                itemType = (CompetitionUpdate.Event.PlayerPickUpEvent.ItemType)pickUpEvent.MineType
                            };
                            break;
                        case PlayerPlaceEventArgs placeEvent:
                            currentEvent.playerPlaceBlockEvent = new()
                            {
                                playerId = placeEvent.Player.PlayerId
                                // TODO: finish the event param
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
            _viewerServer.Publish(new CompetitionUpdate()
            {
                cameras = cameraInfoList,

                chunks = e.Game.GameMap.Chunks.Select(chunk => new CompetitionUpdate.Chunk()
                {
                    chunkId = chunk.Position != null ? chunk.Position.X + chunk.Position.Y * 8 : -1,
                    height = chunk.Height,
                    position = chunk.Position != null ? new CompetitionUpdate.Chunk.Position()
                    {
                        x = chunk.Position.X,
                        y = chunk.Position.Y
                    } : null
                }).ToList(),

                events = currentEvents,

                info = new CompetitionUpdate.Info()
                {
                    stage = e.Game.CurrentStage switch
                    {
                        IGame.Stage.Ready => CompetitionUpdate.Info.Stage.Ready,
                        IGame.Stage.Running => CompetitionUpdate.Info.Stage.Running,
                        IGame.Stage.Ended => CompetitionUpdate.Info.Stage.Ended,
                        IGame.Stage.Finished => CompetitionUpdate.Info.Stage.Finished,
                        IGame.Stage.Battling => CompetitionUpdate.Info.Stage.Battling,
                        _ => throw new NotImplementedException($"{e.Game.CurrentStage} is not implemented")
                    },
                    elapsedTicks = e.Game.ElapsedTicks
                },

                mines = e.Game.Mines.Select(mine => new CompetitionUpdate.Mine()
                {
                    mineId = mine.MineId.ToString(),
                    accumulatedOreCount = mine.AccumulatedOreCount,
                    oreType = (CompetitionUpdate.Mine.OreType)mine.OreKind,
                    position = new CompetitionUpdate.Mine.Position()
                    {
                        x = mine.Position.X,
                        y = mine.Position.Y
                    }
                }).ToList(),

                players = e.Game.Players.Select(player => new CompetitionUpdate.Player()
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
                    homePosition = new CompetitionUpdate.Player.HomePosition()
                    {
                        x = player.SpawnPoint.X,
                        y = player.SpawnPoint.Y,
                    },
                    inventory = new CompetitionUpdate.Player.Inventory()
                    {
                        emerald = player.EmeraldCount,
                        wool = player.WoolCount
                    },
                    position = new CompetitionUpdate.Player.Position()
                    {
                        x = player.PlayerPosition.X,
                        y = player.PlayerPosition.Y
                    }
                }).ToList()
            });
        }
        catch (Exception exception)
        {
            _logger.Warning($"An exception is caught when updating packet: {exception}");
        }
    }



    void HandleAfterJudgementEvent(object? sender, AfterJudgementEventArgs e)
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
