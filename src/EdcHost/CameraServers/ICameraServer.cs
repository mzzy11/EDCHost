namespace EdcHost.CameraServers;

public interface ICameraServer : IDisposable
{
    List<int> AvailableCameraIndexes { get; }

    void CloseCamera(int cameraIndex);

    ICamera GetCamera(int cameraIndex);

    void OpenCamera(int cameraIndex);

    void Start();

    void Stop();
}
