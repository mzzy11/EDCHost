using EdcHost.ViewerServers.EventArgs;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleSetPortEvent(object? sender, SetPortEventArgs e)
    {
        Serilog.Log.Information("[Update]");
        Serilog.Log.Information($"Player {e.PlayerId}:");
        Serilog.Log.Information($"Port: {e.PortName} BaudRate: {e.BaudRate}");
    }

    void HandleSetCameraEvent(object? sender, SetCameraEventArgs e)
    {
        Serilog.Log.Information("[Update]");
        Serilog.Log.Information($"Player {e.PlayerId}:");
        Serilog.Log.Information($"Camera: {e.CameraConfiguration}");
    }
}
