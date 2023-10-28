using Serilog;

namespace EdcHost.CameraServers;

public class CameraServer : ICameraServer
{
    readonly ICameraFactory _cameraFactory;
    readonly List<ICamera> _cameras = new();
    readonly ILocator _locator;
    readonly ILogger _logger = Log.Logger.ForContext("Component", "CameraServers");

    public List<int> AvailableCameraIndexes => _cameraFactory.CameraIndexes;
    public RecognitionOptions Options
    {
        get => _locator.Options;
        set => _locator.Options = value;
    }
    bool _isRunning = false;

    public CameraServer(ICameraFactory cameraFactory, ILocator locator)
    {
        _cameraFactory = cameraFactory;
        _locator = locator;
    }

    public void CloseCamera(int cameraIndex)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        ICamera? camera = _cameras.Find(x => x.CameraIndex == cameraIndex) ??
            throw new ArgumentException($"camera index does not exist: {cameraIndex}");

        camera.Close();
        camera.Dispose();
        _cameras.Remove(camera);
    }

    public ICamera GetCamera(int cameraIndex)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        ICamera? camera = _cameras.Find(x => x.CameraIndex == cameraIndex) ??
            throw new ArgumentException($"camera index does not exist: {cameraIndex}");

        return camera;
    }

    public void OpenCamera(int cameraIndex)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        if (_cameras.Any(x => x.CameraIndex == cameraIndex))
        {
            throw new ArgumentException($"camera index already exists: {cameraIndex}");
        }

        ICamera camera = _cameraFactory.Create(cameraIndex, _locator);
        _cameras.Add(camera);
    }

    public void Start()
    {
        if (_isRunning is true)
        {
            throw new InvalidOperationException("already running");
        }

        _logger.Information("Starting...");

        _isRunning = true;

        _logger.Information("Started.");
    }

    public void Stop()
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        _logger.Information("Stopping...");

        _isRunning = false;

        _logger.Information("Stopped.");
    }

    public void Dispose()
    {
        foreach (ICamera camera in _cameras)
        {
            camera.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
