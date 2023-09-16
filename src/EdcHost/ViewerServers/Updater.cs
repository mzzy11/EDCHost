using EdcHost.ViewerServers.EventArgs;
using EdcHost.ViewerServers.Messages;

namespace EdcHost.ViewerServers;

public class Updater : IUpdater
{
    public IMessage? CachedMessage { get; private set; } = null;
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

    public void UpdateMessage(IMessage message) => CachedMessage = message;

    public void End()
    {
        EndTag = true;
    }
}
