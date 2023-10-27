using SixLabors.ImageSharp.Formats.Jpeg;

namespace EdcHost.Cameras;
public class ImageCompressionAndBase64
{
    public static string? CompressImageToBase64(Image image, long quality)
    {
        try
        {
            using (MemoryStream compressedMs = new MemoryStream())
            {
                image.Save(compressedMs, new JpegEncoder() { Quality = (int)quality });

                byte[] compressedBytes = compressedMs.ToArray();
                string base64String = Convert.ToBase64String(compressedBytes);
                return base64String;
            }
        }
        catch (Exception ex)
        {
            // Error
            Console.WriteLine("Error: " + ex.Message);
            return null;
        }
    }

}