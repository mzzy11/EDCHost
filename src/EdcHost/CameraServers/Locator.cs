using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace EdcHost.CameraServers;

public class Locator : ILocator
{
    public Mat? Mask { get; private set; }

    readonly RecognitionOptions _options;

    public Locator(RecognitionOptions options)
    {
        _options = options;
    }

    public ILocator.RecognitionResult? Locate(Mat originalFrame)
    {
        using Mat mask = GetMask(originalFrame);

        // Show mask if requested.
        if (_options.ShowMask)
        {
            Mask?.Dispose();
            Mask = mask.Clone();
        }

        Tuple<float, float>? location = GetLocation(mask);

        if (location is null)
        {
            return null;
        }

        Tuple<float, float> calibratedLocation = GetCalibratedLocation(location);

        return new ILocator.RecognitionResult
        {
            CalibratedLocation = calibratedLocation,
            Location = location,
        };
    }

    Tuple<float, float> GetCalibratedLocation(Tuple<float, float> pixelLocation)
    {
        using Mat transform = CvInvoke.GetPerspectiveTransform(
            src: new System.Drawing.PointF[] {
                new(_options.TopLeftX, _options.TopLeftY),
                new(_options.TopRightX, _options.TopRightY),
                new(_options.BottomRightX, _options.BottomRightY),
                new(_options.BottomLeftX, _options.BottomLeftY),
            },
            dest: new System.Drawing.PointF[] {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1),
            }
        );

        System.Drawing.PointF[] transformed = CvInvoke.PerspectiveTransform(
            src: new System.Drawing.PointF[] { new(pixelLocation.Item1, pixelLocation.Item2) },
            mat: transform
        );

        Debug.Assert(transformed.Length == 1);

        return new Tuple<float, float>(
            transformed[0].X,
            transformed[0].Y
        );
    }

    Mat GetMask(Mat frame)
    {
        Mat mask = frame.Clone();

        // Convert to HSV color space.
        CvInvoke.CvtColor(
            src: mask,
            dst: mask,
            code: ColorConversion.Bgr2Hsv
        );

        // Binarize the image.
        CvInvoke.InRange(
            src: mask,
            lower: new ScalarArray(new MCvScalar(
                _options.HueCenter - _options.HueRange / 2,
                _options.SaturationCenter - _options.SaturationRange / 2,
                _options.ValueCenter - _options.ValueRange / 2
            )),
            upper: new ScalarArray(new MCvScalar(
                _options.HueCenter + _options.HueRange / 2,
                _options.SaturationCenter + _options.SaturationRange / 2,
                _options.ValueCenter + _options.ValueRange / 2
            )),
            dst: mask
        );

        return mask;
    }

    Tuple<float, float>? GetLocation(Mat mask)
    {
        // Find contours.
        using VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(
            image: mask,
            contours: contours,
            hierarchy: null,
            mode: RetrType.List,
            method: ChainApproxMethod.ChainApproxSimple
        );

        // Find the largest contour.
        int largestContourIndex = 0;
        double largestContourArea = 0;
        for (int i = 0; i < contours.Size; i++)
        {
            double area = CvInvoke.ContourArea(contours[i]);
            if (area > largestContourArea)
            {
                largestContourIndex = i;
                largestContourArea = area;
            }
        }

        // Return null if no area is large enough.
        if (largestContourArea / (mask.Height * mask.Width) < _options.MinArea)
        {
            return null;
        }

        // Find the center of the largest contour.
        using VectorOfPoint largestContour = contours[largestContourIndex];
        using Moments moments = CvInvoke.Moments(largestContour);
        float centerX = (float)(moments.M10 / moments.M00);
        float centerY = (float)(moments.M01 / moments.M00);

        Tuple<float, float> location = new(
            centerX,
            centerY
        );

        return location;
    }
}
