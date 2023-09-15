using EdcHost.ViewerServers.Messages;
using EdcHost.ViewerServers.EventArgs;

namespace EdcHost.ViewerServers;

public interface IUpdater
{
    public IMessage? CachedMessage { get; }
    public bool EndTag { get; }

    public void StartUpdate();
    public void End();
}