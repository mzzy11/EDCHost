using EdcHost.ViewerServers;
using Fleck;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class ViewerServersTests
{
    [Theory]
    [InlineData(100, 8080)]
    public async Task Concurrency(int clientCount, int port)
    {
        WebSocketServerHubMock wsServerHubMock = new()
        {
            Servers = {
                { port, new WebSocketServerMock(port) }
            }
        };
        var viewerServer = new ViewerServer(port, wsServerHubMock);
        viewerServer.Start();
        //do concurrency test
        var tasks = new List<Task>();
        for (int i = 0; i < clientCount; i++)
        {
            tasks.Add(Task.Run(() => {
                var wsServerMock = (WebSocketServerMock)wsServerHubMock.Get(port);
                var wsConnectionMock = new WebSocketConnectionMock();
                wsServerMock.AddConnection(wsConnectionMock);
                wsConnectionMock.OnMessage?.Invoke("{}");
            }));
        }
        await Task.WhenAll(tasks);
        var wsServerMock = (WebSocketServerMock)wsServerHubMock.Get(port);
        //Assertion
        Assert.Equal(clientCount, wsServerMock.Connections.Count);
        viewerServer.Stop();
    }
}