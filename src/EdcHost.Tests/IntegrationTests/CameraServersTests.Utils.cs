using EdcHost.CameraServers;
using Emgu.CV;

namespace EdcHost.Tests.IntegrationTests;

public partial class CameraServersTests
{
    class CameraFactoryMock : ICameraFactory
    {
        public Dictionary<int, CameraMock> Cameras = new();

        public List<int> CameraIndexes => Cameras.Keys.ToList();

        public ICamera Create(int cameraIndex, ILocator locator)
        {
            if (!Cameras.ContainsKey(cameraIndex))
            {
                throw new ArgumentException($"camera index does not exist: {cameraIndex}");
            }

            return Cameras[cameraIndex];
        }

        public void Scan()
        {
        }
    }

    class CameraMock : ICamera
    {
        public bool IsDisposed = false;
        public bool IsOpen = false;
        private Mat _frame = new();

        public CameraMock(int cameraIndex)
        {
            CameraIndex = cameraIndex;
        }

        public void UpdateFrame(Mat frame)
        {
            _frame.Dispose();
            _frame = frame;

            ILocator.RecognitionResult? result = Locator.Locate(_frame);

            if (result is not null)
            {
                TargetLocation = result.CalibratedLocation;
                TargetLocationNotCalibrated = result.Location;
            }
        }

        public int CameraIndex { get; }
        public int Height => _frame.Height;
        public bool IsOpened => IsOpen;
        public byte[]? JpegData { get; }
        public ILocator Locator { get; set; } = new Locator();
        public int Width => _frame.Width;
        public Tuple<float, float>? TargetLocation { get; private set; } = null;
        public Tuple<float, float>? TargetLocationNotCalibrated { get; private set; } = null;

        public void Close()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException();
            }

            IsOpen = false;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void Open()
        {
            if (IsOpen)
            {
                throw new InvalidOperationException();
            }

            IsOpen = true;
        }
    }
}
