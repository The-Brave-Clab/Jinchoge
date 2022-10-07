using System;
using System.Collections.Generic;
using System.Linq;

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
        { TaskType.Id, "ID" },
        { TaskType.Header, "Common Data" },
        { TaskType.Profile, "Profile" },
        { TaskType.Accessories, "Spirits" },
        { TaskType.Cards, "Cards" },
        { TaskType.Decks, "Teams" },
        { TaskType.EnhancementItems, "Enhancement Items" },
        { TaskType.EventItems, "Event Items" },
        { TaskType.EvolutionItems, "Evolution Items" },
        { TaskType.StaminaItems, "Stamina Items" },
        { TaskType.TitleItems, "Titles" },
        { TaskType.CharacterFamiliarities, "Character Familiarities" }
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