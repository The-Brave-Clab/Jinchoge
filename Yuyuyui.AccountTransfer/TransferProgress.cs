using System;
using System.Collections.Generic;
using System.Linq;
using Yuyuyui.PrivateServer.Localization;

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

    public static Dictionary<TaskType, string> TaskName => new()
    {
        { TaskType.Id, Resources.AT_TASK_ID },
        { TaskType.Header, Resources.AT_TASK_HEADER },
        { TaskType.Profile, Resources.AT_TASK_PROFILE },
        { TaskType.Accessories, Resources.AT_TASK_ACCESSORIES },
        { TaskType.Cards, Resources.AT_TASK_CARDS },
        { TaskType.Decks, Resources.AT_TASK_DECKS },
        { TaskType.EnhancementItems, Resources.AT_TASK_ITEMS_ENHANCEMENT },
        { TaskType.EventItems, Resources.AT_TASK_ITEMS_EVENT },
        { TaskType.EvolutionItems, Resources.AT_TASK_ITEMS_EVOLUTION },
        { TaskType.StaminaItems, Resources.AT_TASK_ITEMS_STAMINA },
        { TaskType.TitleItems, Resources.AT_TASK_ITEMS_TITLE },
        { TaskType.CharacterFamiliarities, Resources.AT_TASK_CHARACTER_FAMILIARITIES }
    };

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