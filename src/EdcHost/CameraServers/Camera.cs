using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using Serilog;

namespace EdcHost.CameraServers;

public class Camera : ICamera
{
    readonly ILocator _locator;
    readonly ILogger _logger = Log.Logger.ForContext("Component", "CameraServers");

    public int CameraIndex { get; private set; }
    public int Height => _capture.Height;
    public bool IsOpened => _capture.IsOpened;
    public byte[]? JpegData { get; private set; }
    public Tuple<float, float>? TargetPosition { get; private set; }
    public Tuple<float, float>? TargetPositionNotCalibrated { get; private set; }
    public int Width => _capture.Width;

    VideoCapture _capture;
    Task? _task = null;
    CancellationTokenSource? _taskCancellationTokenSource = null;

    public Camera(int cameraIndex, ILocator locator)
    {
        CameraIndex = cameraIndex;
        _capture = new(cameraIndex);
        _locator = locator;
    }

    public void Close()
    {
        if (!_capture.IsOpened)
        {
            throw new InvalidOperationException("camera is not open");
        }

        Debug.Assert(_task is not null);
        Debug.Assert(_taskCancellationTokenSource is not null);

        _capture.Dispose();

        _taskCancellationTokenSource.Cancel();
        _task.Wait();
        _task.Dispose();
    }

    public void Dispose()
    {
        _capture.Dispose();
        _task?.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Open()
    {
        if (_capture.IsOpened)
        {
            throw new InvalidOperationException("camera is already open");
        }

        Debug.Assert(_task is null);
        Debug.Assert(_taskCancellationTokenSource is null);

        _capture = new(CameraIndex);

        _task = Task.Run(TaskForCapturingFunc);
        _taskCancellationTokenSource = new();
    }

    async Task TaskForCapturingFunc()
    {
        Debug.Assert(JpegData is null);
        Debug.Assert(TargetPosition is null);
        Debug.Assert(TargetPositionNotCalibrated is null);

        while (!_taskCancellationTokenSource?.Token.IsCancellationRequested ?? false)
        {
            await Task.Delay(0);

            using Mat frame = _capture.QueryFrame();

            ILocator.RecognitionResult? recognitionResult = _locator.Locate(frame);

            if (_locator.Mask is null)
            {
                JpegData = frame.ToImage<Bgr, byte>().ToJpegData();
            }
            else
            {
                JpegData = _locator.Mask.ToImage<Bgr, byte>().ToJpegData();
            }

            if (recognitionResult is null)
            {
                TargetPosition = null;
                TargetPositionNotCalibrated = null;
            }
            else
            {
                TargetPosition = recognitionResult.Location;
                TargetPositionNotCalibrated = recognitionResult.CalibratedLocation;
            }
        }

        JpegData = null;
        TargetPosition = null;
        TargetPositionNotCalibrated = null;
    }
}
