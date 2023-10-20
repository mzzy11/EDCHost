using System.Drawing;
using OpenCvSharp;

namespace EdcHost.Cameras;

public interface ICamera
{
    static ICamera Create(int cameraIndex)
    {
        VideoCapture videoCapture = new(cameraIndex);
        return new Camera(videoCapture);
    }

    Image? Image { get; }
    IPosition<int>? TargetPosition { get; }
}
