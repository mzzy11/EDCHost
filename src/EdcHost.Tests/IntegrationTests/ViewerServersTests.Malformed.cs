using System.Text;
using EdcHost.ViewerServers;
using Fleck;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class ViewerServersTests
{
    [Theory]
    [InlineData("", 8080)]
    void Malformed(string message, int port)
    {
        // Arrange
        WebSocketServerHubMock wsServerHubMock = new()
        {
            Servers = {
                { port, new WebSocketServerMock(port) }
            }
        };
        ViewerServer viewerServer = new(port, wsServerHubMock);
        viewerServer.Start();

        WebSocketServerMock wsServerMock = (WebSocketServerMock)wsServerHubMock.Get(port);
        WebSocketConnectionMock wsConnectionMock = new();
        wsServerMock.AddConnection(wsConnectionMock);

        // Act
        wsConnectionMock.OnMessage?.Invoke(message);

        // Assertion
        Exception exception = Assert.Throws<Exception>(() => viewerServer.Stop());
    }
}