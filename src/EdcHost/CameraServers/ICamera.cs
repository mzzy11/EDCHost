using System.Drawing;

namespace EdcHost.CameraServers;

public interface ICamera : IDisposable
{
    System.Drawing.Image? Frame { get; }
    Tuple<float, float>? TargetPosition { get; }

    void Open();
    void Close();
}
