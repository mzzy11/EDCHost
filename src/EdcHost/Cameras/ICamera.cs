using System.Drawing;

namespace EdcHost.Cameras;

public interface ICamera
{
    Image Image { get; }
    IPosition<int> TargetPosition { get; }
}
