using EdcHost.ViewerServers.Messages;
using EdcHost.ViewerServers.EventArgs;

namespace EdcHost.ViewerServers;

public class Updater : IUpdater
{
    public IMessage? CachedMessage { get; } = null;
    public bool EndTag { get; private set; }
    public event EventHandler<MessageTransferEventArgs>? SendCaller = null;

    public async void StartUpdate()
    {
        EndTag = false;
        await Task.Run(() => 
        {
            while (true)
            {
                Task.Delay(100).Wait();

                if (EndTag == true)
                    return;

                if (CachedMessage == null)
                    continue;

                SendCaller?.Invoke(this, new MessageTransferEventArgs(CachedMessage));
            }
        });
    }

    public void End()
    {
        EndTag = true;
    }
}