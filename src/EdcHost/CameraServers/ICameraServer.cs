namespace EdcHost.CameraServers;

public interface ICameraServer : IDisposable
{
    static ICameraServer Create()
    {
        return new CameraServer(new CameraFactory(), new Locator());
    }

    List<int> AvailableCameraIndexes { get; }

    RecognitionOptions Options { get; set; }

    void CloseCamera(int cameraIndex);

    ICamera GetCamera(int cameraIndex);

    void OpenCamera(int cameraIndex);

    void Start();

    void Stop();
}
