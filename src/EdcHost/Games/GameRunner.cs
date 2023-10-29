using System.Diagnostics;

namespace EdcHost.Games;

class GameRunner : IGameRunner
{
    const int TicksPerSecondExpected = 20;

    public bool IsRunning { get; private set; } = false;

    public IGame Game { get; }

    Task? _task = null;

    public GameRunner(IGame game)
    {
        Game = game;
    }

    public void Start()
    {
        if (Game.CurrentStage is not IGame.Stage.Ready)
        {
            throw new InvalidOperationException("game is not ready");
        }

        IsRunning = true;

        Game.Start();

        _task = TaskFunc();
    }

    public void End()
    {
        if (Game.CurrentStage is not IGame.Stage.Running && Game.CurrentStage is not IGame.Stage.Battling)
        {
            throw new InvalidOperationException("game is not running");
        }

        Debug.Assert(_task is not null);

        IsRunning = false;
        _task.Wait();
    }

    async Task TaskFunc()
    {
        DateTime lastTickStartTime = DateTime.Now;

        while (IsRunning)
        {
            if (Game.CurrentStage is not IGame.Stage.Running && Game.CurrentStage is not IGame.Stage.Battling)
            {
                break;
            }

            // Wait for next tick
            DateTime currentTickStartTime = lastTickStartTime.AddMilliseconds((double)1000 / TicksPerSecondExpected);
            if (currentTickStartTime > DateTime.Now)
            {
                await Task.Delay(currentTickStartTime - DateTime.Now);
            }
            else
            {
                currentTickStartTime = DateTime.Now;
            }

            Game.Tick();

            lastTickStartTime = currentTickStartTime;
        }
    }
}
