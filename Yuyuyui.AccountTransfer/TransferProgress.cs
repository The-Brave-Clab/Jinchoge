using YamlDotNet.Core.Tokens;

namespace Yuyuyui.AccountTransfer;

public static class TransferProgress
{

    public enum TaskType
    {
        UUID,
        Code,
        Header,
        Profile,
        
        Count
    }

    private static readonly EventWaitHandle waitHandle = new AutoResetEvent(false);

    public static void WaitForCompletion()
    {
        waitHandle.WaitOne();
    }

    private static bool[] transferStatus = null;

    static TransferProgress()
    {
        transferStatus = new bool[(int) TaskType.Count];
        for (int i = 0; i < (int) TaskType.Count; ++i)
            transferStatus[i] = false;
    }

    public static void Completed(TaskType taskType)
    {
        transferStatus[(int) taskType] = true;
        if (transferStatus.All(b => b))
            waitHandle.Set();
    }
}