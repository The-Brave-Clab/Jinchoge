using YamlDotNet.Core.Tokens;

namespace Yuyuyui.AccountTransfer;

public static class TransferProgress
{

    public enum TaskType
    {
        Id,
        Header,
        Profile,
        Accessories,
        Cards,
        Decks,
        
        // Items
        EnhancementItems,
        EventItems,
        EvolutionItems,
        StaminaItems,
        TitleItems,
        
        CharacterFamiliarities,
        
        Count_DoNotUse
    }

    private static readonly EventWaitHandle waitHandle = new AutoResetEvent(false);

    public static void WaitForCompletion()
    {
        waitHandle.WaitOne();
    }

    private static bool[] transferStatus;

    static TransferProgress()
    {
        int taskCount = (int) TaskType.Count_DoNotUse;
        transferStatus = new bool[taskCount];
        for (int i = 0; i < taskCount; ++i)
            transferStatus[i] = false;
    }

    public static void Complete(TaskType taskType)
    {
        transferStatus[(int) taskType] = true;
        if (transferStatus.All(b => b))
            waitHandle.Set();
    }

    public static bool IsCompleted(TaskType taskType)
    {
        return transferStatus[(int)taskType];
    }
}