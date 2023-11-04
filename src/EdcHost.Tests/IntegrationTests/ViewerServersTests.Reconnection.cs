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
        viewerServer.Start();

        var wsServerMock = (WebSocketServerMock)wsServerHubMock.Get(port);
        var wsConnectionMock = new WebSocketConnectionMock();
        wsServerMock.AddConnection(wsConnectionMock);
        wsConnectionMock.OnMessage?.Invoke("{}");
        wsConnectionMock.OnClose?.Invoke();
        
        //Assertion
        Assert.Empty(wsServerMock.Connections);

        viewerServer.Stop();
    }
}