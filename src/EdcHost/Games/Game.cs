namespace EdcHost.Games;

public partial class Game : IGame
{
    //TODO: Seperate class Game

    //TODO: Add events to Game
    //TODO: Add fields and properties related to time and game tick
    //TODO: Add other fields and properties

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
    /// Constructor
    /// </summary>
    public Game()
    {
        //TODO: Initialize properties
        _map = new Map();
        _players = new();
        _mines = new();
        GenerateMines();
        //TODO: Add players
        //TODO: Subscribe player events
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
                //TODO: add lock
                //TODO: Handle ticks

                UpdateMap();
                UpdateMines();
                UpdatePlayerInfo();
                UpdateGameStage();

                //TODO: Terminate game when finished
                //TODO: Invoke game events
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
    /// Update game stage.
    /// </summary>
    private void UpdateGameStage()
    {
        //TODO: Update game stage
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
    /// Handle PlayerMoveEvent
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event args</param>
    private void PlayerMoveEventHandler(object sender, PlayerMoveEventArgs e)
    {
        //TODO: Handle PlayerMoveEvent
    }

    //TODO: Create more player event handler

}
