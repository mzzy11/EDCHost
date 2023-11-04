using EdcHost.ViewerServers;
using Fleck;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class ViewerServersTests
{
    [Theory]
    [InlineData(8080)]
    public void Reconnection(int port)
    {
        WebSocketServerHubMock wsServerHubMock = new()
        {
            Servers = {
                { port, new WebSocketServerMock(port) }
            }
        };
        var viewerServer = new ViewerServer(port, wsServerHubMock);
        var wsServerMock = (WebSocketServerMock)wsServerHubMock.Get(port);
        var wsConnectionMock = new WebSocketConnectionMock();

        viewerServer.Start();
        wsServerMock.AddConnection(wsConnectionMock);

        //Assertion
        Assert.Single(wsServerMock.Connections);

        wsConnectionMock.OnClose?.Invoke();

        //Assertion
        Assert.Empty(wsServerMock.Connections);

        wsConnectionMock.OnOpen?.Invoke();

        //Assertion
        Assert.Single(wsServerMock.Connections);

        viewerServer.Stop();
    }
}
