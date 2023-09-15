namespace EdcHost.Games;

public partial class Game : IGame
{
    //TODO: Seperate class Game

    //TODO: Add events to Game
    //TODO: Add other fields and properties

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
    /// When will Battling stage start.
    /// </summary>
    private readonly TimeSpan _startBattlingTime = TimeSpan.FromSeconds(600);

    /// <summary>
    /// How nuch time a player should wait until respawn.
    /// </summary>
    private readonly TimeSpan _respawnTimeInterval = TimeSpan.FromSeconds(15);

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
        _map = new Map();
        _players = new(2);
        _mines = new();
        _tickTask = new(Tick);
        GenerateMines();
        //TODO: Add players
        //TODO: Subscribe player events
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Start()
    {
        if (CurrentStage == IGame.Stage.Running || CurrentStage == IGame.Stage.Battling)
        {
            throw new InvalidOperationException("The game is already started.");
        }

        //TODO: Start game after all players are ready

        CurrentStage = IGame.Stage.Running;
        ElapsedTime = TimeSpan.FromSeconds(0);
        Winner = null;

        _tickTask.Start();

        //TODO: Invoke game events
    }

    /// <summary>
    /// Stops the game.
    /// </summary>
    public void Stop()
    {
        if (CurrentStage == IGame.Stage.Ready)
        {
            Serilog.Log.Warning("The game has not started yet.");
        }
        lock (this)
        {
            Judge();
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
                    if (CurrentStage == IGame.Stage.Ready
                        || CurrentStage == IGame.Stage.Finished)
                    {
                        break;
                    }

                    //TODO: Handle time and ticks

                    UpdateMap();
                    UpdateMines();
                    UpdatePlayerInfo();
                    UpdateGameStage();

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
    /// Update game stage.
    /// </summary>
    private void UpdateGameStage()
    {
        if (CurrentStage == IGame.Stage.Ready || CurrentStage == IGame.Stage.Finished)
        {
            throw new InvalidOperationException("The game is not running.");
        }
        if (IsFinished())
        {
            CurrentStage = IGame.Stage.Finished;
        }
        else if (ElapsedTime >= _startBattlingTime)
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
        //TODO: Accumulate ore automaticly
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

    //TODO: Create more player event handler

}
