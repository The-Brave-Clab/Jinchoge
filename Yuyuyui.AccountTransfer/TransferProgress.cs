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
        Accessories,
        Cards,
        Decks,
        
        // Items
        AutoClearTickets,
        EnhancementItems,
        EventItems,
        EvolutionItems,
        StaminaItems,
        TitleItems,
        
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
        int taskCount = (int) TaskType.Count;
        transferStatus = new bool[taskCount];
        for (int i = 0; i < taskCount; ++i)
            transferStatus[i] = false;
    }

    public static void Completed(TaskType taskType)
    {
        transferStatus[(int) taskType] = true;
        if (transferStatus.All(b => b))
            waitHandle.Set();
    }
}