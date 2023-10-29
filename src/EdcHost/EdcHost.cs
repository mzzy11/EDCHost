using System.Diagnostics;
using System.Collections.Concurrent;
using Serilog;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    const int MapHeight = 8;
    const int MapWidth = 8;

    readonly ILogger _logger = Log.ForContext("Component", "EdcHost");
    readonly ConcurrentQueue<EventArgs> _playerEventQueue = new();
    readonly ConcurrentDictionary<int, PlayerHardwareInfo> _playerHardwareInfo = new();
    readonly CameraServers.ICameraServer _cameraServer;
    readonly SlaveServers.ISlaveServer _slaveServer;
    readonly ViewerServers.IViewerServer _viewerServer;
    readonly Config _config;

    public bool IsRunning { get; private set; } = false;

    Games.IGame _game;
    Games.IGameRunner _gameRunner;
    Task? _task = null;
    CancellationTokenSource? _taskCancellationTokenSource = null;

    public EdcHost(Config config)
    {
        _config = config;

        _game = Games.IGame.Create(
            diamondMines: _config.Game.DiamondMines,
            goldMines: _config.Game.GoldMines,
            ironMines: _config.Game.IronMines
        );
        _gameRunner = Games.IGameRunner.Create(_game);
        _cameraServer = CameraServers.ICameraServer.Create();
        _slaveServer = SlaveServers.ISlaveServer.Create();
        _viewerServer = ViewerServers.IViewerServer.Create(_config.ServerPort);

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

        _viewerServer.AfterMessageReceiveEvent += HandleAfterMessageReceiveEvent;
    }

    public void Start()
    {
        _logger.Information("Starting...");

        Debug.Assert(_task is null);
        Debug.Assert(_taskCancellationTokenSource is null);

        _cameraServer.Start();
        _slaveServer.Start();
        _viewerServer.Start();

        _taskCancellationTokenSource = new CancellationTokenSource();
        _task = Task.Run(TaskFunc);

        IsRunning = true;

        _logger.Information("Started.");
    }

    public void Stop()
    {
        _logger.Information("Stopping...");

        Debug.Assert(_task is not null);
        Debug.Assert(_taskCancellationTokenSource is not null);

        _taskCancellationTokenSource.Cancel();
        _task.Wait();
        _taskCancellationTokenSource.Dispose();
        _task.Dispose();

        _cameraServer.Stop();
        _slaveServer.Stop();
        _viewerServer.Stop();

        IsRunning = false;

        _logger.Information("Stopped.");
    }

    void TaskFunc()
    {
        while (!_taskCancellationTokenSource?.Token.IsCancellationRequested ?? false)
        {
            List<int> heightOfChunks = new();
            foreach (Games.IChunk chunk in _game.GameMap.Chunks)
            {
                heightOfChunks.Add(chunk.Height);
            }

            for (int i = 0; i < 2; i++)
            {
                string? portName = _playerHardwareInfo.GetValueOrDefault(_game.Players[i].PlayerId).PortName;
                if (portName is null)
                {
                    continue;
                }

                _slaveServer.Publish(
                    portName: portName,
                    gameStage: (int)_game.CurrentStage,
                    elapsedTime: _game.ElapsedTicks,
                    heightOfChunks: heightOfChunks,
                    hasBed: _game.Players[i].HasBed,
                    hasBedOpponent: _game.Players.Any(player => player.HasBed && player.PlayerId != _game.Players[i].PlayerId),
                    positionX: _game.Players[i].PlayerPosition.X,
                    positionY: _game.Players[i].PlayerPosition.Y,
                    positionOpponentX: _game.Players[(i == 0) ? 1 : 0].PlayerPosition.X,
                    positionOpponentY: _game.Players[(i == 0) ? 1 : 0].PlayerPosition.Y,
                    agility: _game.Players[i].ActionPoints,
                    health: _game.Players[i].Health,
                    maxHealth: _game.Players[i].MaxHealth,
                    strength: _game.Players[i].Strength,
                    emeraldCount: _game.Players[i].EmeraldCount,
                    woolCount: _game.Players[i].WoolCount
                );
            }

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

                chunks = _game.GameMap.Chunks.Select(chunk => new ViewerServers.CompetitionUpdateMessage.Chunk()
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
                    stage = _game.CurrentStage switch
                    {
                        Games.IGame.Stage.Ready => ViewerServers.CompetitionUpdateMessage.Info.Stage.Ready,
                        Games.IGame.Stage.Running => ViewerServers.CompetitionUpdateMessage.Info.Stage.Running,
                        Games.IGame.Stage.Ended => ViewerServers.CompetitionUpdateMessage.Info.Stage.Ended,
                        Games.IGame.Stage.Finished => ViewerServers.CompetitionUpdateMessage.Info.Stage.Finished,
                        Games.IGame.Stage.Battling => ViewerServers.CompetitionUpdateMessage.Info.Stage.Battling,
                        _ => throw new NotImplementedException($"{_game.CurrentStage} is not implemented")
                    },
                    elapsedTicks = _game.ElapsedTicks
                },

                mines = _game.Mines.Select(mine => new ViewerServers.CompetitionUpdateMessage.Mine()
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

                players = _game.Players.Select(player => new ViewerServers.CompetitionUpdateMessage.Player()
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
    }
}
