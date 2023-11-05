using EdcHost.CameraServers;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class CameraServersTests
{
    [Fact]
    public void Simple()
    {
        const int CameraIndex = 0;

        // Arrange
        CameraMock cameraMock = new(CameraIndex);
        CameraFactoryMock cameraFactoryMock = new()
        {
            Cameras = {
                { CameraIndex, cameraMock }
            }
        };
        ICameraServer cameraServer = new CameraServer(cameraFactoryMock);
        ILocator locator = new Locator();

        // Act
        cameraServer.Start();
        cameraServer.OpenCamera(CameraIndex, locator);
    }
}
