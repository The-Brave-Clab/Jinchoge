using System;
using System.Linq;
using System.Threading;

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

    private static Action<TaskType, bool[]>? singleCompleteCallback = null;
    private static Action? allCompleteCallback = null;

    private static bool[] transferStatus;

    static TransferProgress()
    {
        int taskCount = (int) TaskType.Count_DoNotUse;
        transferStatus = new bool[taskCount];
        for (int i = 0; i < taskCount; ++i)
            transferStatus[i] = false;
    }

    public static void RegisterTaskCompleteCallback(Action<TaskType, bool[]>? callback)
    {
        singleCompleteCallback = callback;
    }

    public static void RegisterAllTaskCompleteCallback(Action callback)
    {
        allCompleteCallback = callback;
    }

    public static void Complete(TaskType taskType)
    {
        transferStatus[(int) taskType] = true;
        singleCompleteCallback?.Invoke(taskType, transferStatus);
        if (transferStatus.All(b => b))
            allCompleteCallback?.Invoke();
    }

    public static bool IsCompleted(TaskType taskType)
    {
        return transferStatus[(int)taskType];
    }
}