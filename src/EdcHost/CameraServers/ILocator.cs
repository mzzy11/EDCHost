using Emgu.CV;

namespace EdcHost.CameraServers;

public interface ILocator
{
    Tuple<float, float> Locate(Mat frame);
}
