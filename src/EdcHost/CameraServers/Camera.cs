using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace EdcHost.CameraServers;

public class Camera : ICamera
{
    readonly VideoCapture _capture;

    public int CameraIndex { get; private set; }
    public byte[]? JpegData { get; private set; }
    public Tuple<float, float>? TargetPosition { get; private set; }
    public Tuple<float, float>? TargetPositionNotCalibrated { get; private set; }

    Task? _task = null;

    public Camera(int cameraIndex)
    {
        CameraIndex = cameraIndex;
        _capture = new(cameraIndex);
    }

    public void Close()
    {
        if (!_capture.IsOpened)
        {
            throw new InvalidOperationException("camera is not open");
        }

        Debug.Assert(_task != null);

        _capture.Start();

        _task.Wait();
        _task.Dispose();
    }

    public void Dispose()
    {
        _capture.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Open()
    {
        if (_capture.IsOpened)
        {
            throw new InvalidOperationException("camera is already open");
        }

        _capture.Start();

        _task = Task.Run(TaskForCapturingFunc);
    }

    async Task TaskForCapturingFunc()
    {
        while (_capture.IsOpened)
        {
            await Task.Delay(0);

            using Mat frame = _capture.QueryFrame();
            if (frame == null)
            {
                JpegData = null;
                continue;
            }

            JpegData = frame.ToImage<Bgr, byte>().ToJpegData();
        }
    }
}
