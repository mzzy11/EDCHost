namespace EdcHost.CameraServers;

public interface ICameraHub
{
    List<int> CameraIndexes { get; }

    ICamera Get(int cameraIndex);
}
