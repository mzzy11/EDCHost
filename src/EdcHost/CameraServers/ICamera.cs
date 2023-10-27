using System.Drawing;

namespace EdcHost.CameraServers;

public interface ICamera : IDisposable
{
    int CameraIndex { get; }
    byte[]? JpegData { get; }
    Tuple<float, float>? TargetPosition { get; }
    Tuple<float, float>? TargetPositionNotCalibrated { get; }

    void Close();
    void Open();
}
