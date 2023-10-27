namespace EdcHost.CameraServers;

public record RecognitionOptions
{
    public float TopLeftX { get; init; }
    public float TopLeftY { get; init; }
    public float TopRightX { get; init; }
    public float TopRightY { get; init; }
    public float BottomLeftX { get; init; }
    public float BottomLeftY { get; init; }
    public float BottomRightX { get; init; }
    public float BottomRightY { get; init; }
    public float HueCenter { get; init; }
    public float HueRange { get; init; }
    public float SaturationCenter { get; init; }
    public float SaturationRange { get; init; }
    public float ValueCenter { get; init; }
    public float ValueRange { get; init; }
    public float MinArea { get; init; }
    public bool ShowMask { get; init; }
}
