using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class GamesTests
{
    [Fact]
    public void Simple()
    {
        var game = IGame.Create();

        IGame.Stage? stage = game.CurrentStage;
        Assert.StrictEqual(IGame.Stage.Ready, stage);

        game.Start();

        stage = game.CurrentStage;
        Assert.StrictEqual(IGame.Stage.Running, stage);

        game.End();

        stage = game.CurrentStage;
        Assert.StrictEqual(IGame.Stage.Ended, stage);
    }
}
