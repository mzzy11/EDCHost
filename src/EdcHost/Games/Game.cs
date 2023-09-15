namespace EdcHost.Games;

/// <summary>
/// Game handles the game logic.
/// </summary>
public partial class Game : IGame
{
    //TODO: Seperate class Game

    //TODO: Add events to Game
    //TODO: Add other fields and properties

    /// <summary>
    /// How many seconds one tick takes.
    /// </summary>
    private const decimal SecondsPerTick = 0.05M;

    /// <summary>
    /// Maximum count of same type of items a player can hold.
    /// </summary>
    private const int MaximumItemCount = 64;

    /// <summary>
    /// When will Battling stage start.
    /// </summary>
    private readonly TimeSpan StartBattlingTime = TimeSpan.FromSeconds(600);

    /// <summary>
    /// How much time a player should wait until respawn.
    /// </summary>
    private readonly TimeSpan RespawnTimeInterval = TimeSpan.FromSeconds(15);

    /// <summary>
    /// How much time required to generate ore.
    /// </summary>
    private readonly TimeSpan AccumulateOreInterval = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Current stage of the game.
    /// </summary>
    public IGame.Stage CurrentStage { get; private set; }

    /// <summary>
    /// Elapsed time of the game.
    /// </summary>
    public TimeSpan ElapsedTime { get; private set; }

    /// <summary>
    /// Winner of the game.
    /// </summary>
    /// <remarks>
    /// Winner can be null in case there are no winner.
    /// </remarks>
    public IPlayer? Winner { get; private set; }

    /// <summary>
    /// When game starts.
    /// </summary>
    private DateTime? _startTime;

    /// <summary>
    /// Last tick.
    /// </summary>
    private DateTime? _lastTickTime;

    /// <summary>
    /// Last time ore generated.
    /// </summary>
    private DateTime? _lastOreGeneratedTime;

    /// <summary>
    /// The game map.
    /// </summary>
    private readonly IMap _map;

    /// <summary>
    /// All players.
    /// </summary>
    private readonly List<IPlayer> _players;

    /// <summary>
    /// All mines.
    /// </summary>
    private readonly List<IMine> _mines;

    /// <summary>
    /// The tick task.
    /// </summary>
    private readonly Task _tickTask;

    /// <summary>
    /// Constructor
    /// </summary>
    public Game()
    {
        CurrentStage = IGame.Stage.Ready;
        ElapsedTime = TimeSpan.FromSeconds(0);
        Winner = null;

        _startTime = null;
        _lastTickTime = null;
        _lastOreGeneratedTime = null;

        _map = new Map();
        _players = new(2);
        _mines = new();
        GenerateMines();

        _tickTask = new(Tick);
        //TODO: Add players
        //TODO: Subscribe player events
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Start()
    {
        if (_startTime is not null)
        {
            throw new InvalidOperationException("The game is already started.");
        }

        //TODO: Start game after all players are ready

        CurrentStage = IGame.Stage.Running;
        Winner = null;

        DateTime initTime = DateTime.Now;
        _startTime = initTime;
        _lastTickTime = initTime;
        _lastOreGeneratedTime = initTime;
        ElapsedTime = TimeSpan.FromSeconds(0);

        _tickTask.Start();

        //TODO: Invoke game events
    }

    /// <summary>
    /// Stops the game.
    /// </summary>
    public void Stop()
    {
        if (_startTime is null)
        {
            Serilog.Log.Warning("The game has not started yet.");
        }
        lock (this)
        {
            Judge();
            _startTime = null;
            _lastTickTime = null;
            _lastOreGeneratedTime = null;
            ElapsedTime = TimeSpan.FromSeconds(0);
        }
        _tickTask.Wait();
    }

    /// <summary>
    /// Ticks the game.
    /// </summary>
    public void Tick()
    {
        while (true)
        {
            try
            {
                lock (this)
                {
                    if (_startTime is null)
                    {
                        break;
                    }

                    //TODO: Handle time and ticks
                    ElapsedTime = DateTime.Now - (DateTime)_startTime;

                    Update();

                    if (CurrentStage == IGame.Stage.Finished)
                    {
                        Stop();
                    }

                    //TODO: Invoke game events
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"An exception has been caught: {e}");
            }
        }
    }

    /// <summary>
    /// Generate mines when game start.
    /// </summary>
    private void GenerateMines()
    {
        //TODO: Generate mines according to game rule
    }

    /// <summary>
    /// Whether all players are ready.
    /// </summary>
    /// <returns>True if all reasy, false otherwise.</returns>
    private bool IsReady()
    {
        //TODO: Check whether all players are ready
        return true;
    }

    /// <summary>
    /// Update game.
    /// </summary>
    private void Update()
    {
        if (_startTime is null)
        {
            throw new InvalidOperationException("The game is not running.");
        }
        UpdateMap();
        UpdateMines();
        UpdatePlayerInfo();
        UpdateGameStage();
    }

    /// <summary>
    /// Update game stage.
    /// </summary>
    private void UpdateGameStage()
    {
        if (IsFinished())
        {
            CurrentStage = IGame.Stage.Finished;
        }
        else if (ElapsedTime >= StartBattlingTime)
        {
            CurrentStage = IGame.Stage.Battling;
        }
        else
        {
            CurrentStage = IGame.Stage.Running;
        }
    }

    /// <summary>
    /// Update player infomation.
    /// </summary>
    private void UpdatePlayerInfo()
    {
        //TODO: Update player infomation
    }

    /// <summary>
    /// Update mines.
    /// </summary>
    private void UpdateMines()
    {
        if (_lastOreGeneratedTime is null)
        {
            throw new NullReferenceException("_lastOreGeneratedTime is null");
        }

        DateTime currentTime = DateTime.Now;
        if (currentTime - (DateTime)_lastOreGeneratedTime >= AccumulateOreInterval)
        {
            foreach (Mine mine in _mines)
            {
                mine.Generate();
            }
            _lastOreGeneratedTime = currentTime;
        }

        //TODO: Update AccumulatedOreCount when a player collects ore
    }

    /// <summary>
    /// Update game map
    /// </summary>
    private void UpdateMap()
    {
        //TODO: Update game map
    }

    /// <summary>
    /// Whether the game is finished or not
    /// </summary>
    /// <returns>True if finished, false otherwise.</returns>
    private bool IsFinished()
    {
        //TODO: Check whether the game is finished or not
        return false;
    }

    /// <summary>
    /// Judge the game. Choose a winner or report there is no winner.
    /// </summary>
    private void Judge()
    {
        //TODO: Judge the game
    }

    /// <summary>
    /// Handle PlayerMoveEvent
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event args</param>
    private void PlayerMoveEventHandler(object? sender, PlayerMoveEventArgs e)
    {
        //TODO: Handle PlayerMoveEvent
    }

    //TODO: Add more player event handler
    //TODO: Add methods to calculate values based on players' properties

}
