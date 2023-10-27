using Emgu.CV;

namespace EdcHost.CameraServers;

public class CameraFactory : ICameraFactory
{
    const int MaxNotWorkingCount = 10;
    readonly Task _task;
    readonly CancellationTokenSource _taskCancellationTokenSource;

    public List<int> CameraIndexes { get; } = new();

    public CameraFactory()
    {
        _task = Task.Run(TaskForCameraIndexesFunc);
        _taskCancellationTokenSource = new();
    }

    ~CameraFactory()
    {
        _taskCancellationTokenSource.Cancel();
        _task.Wait();
    }

    public ICamera Create(int cameraIndex, ILocator locator)
    {
        if (!CameraIndexes.Contains(cameraIndex))
        {
            throw new ArgumentException($"Camera index {cameraIndex} is not available");
        }

        return new Camera(cameraIndex, locator);
    }

    static bool TestCamera(int cameraIndex)
    {
        using VideoCapture capture = new(cameraIndex);

        if (!capture.IsOpened)
        {
            return false;
        }

        using Mat frame = capture.QueryFrame();

        if (frame.IsEmpty)
        {
            return false;
        }

        return true;
    }

    void TaskForCameraIndexesFunc()
    {
        while (_taskCancellationTokenSource.Token.IsCancellationRequested is false)
        {
            List<int> cameraIndexes = new();
            int index = 0;
            int notWorkingCount = 0;
            while (notWorkingCount <= MaxNotWorkingCount)
            {
                if (TestCamera(index))
                {
                    cameraIndexes.Add(index);
                }
                else
                {
                    ++notWorkingCount;
                }

                ++index;
            }
        }
    }
}
