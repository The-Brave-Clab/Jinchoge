namespace Yuyuyui.PrivateServer;

public enum ItemCategory
{
    None,
    Card,
    Accessory,
    EnhancementItem,
    EvolutionItem,
    StaminaItem,
    MiscItem,
    RoomItem, // wow
    CharacterScenario,
    CharacterExtraScenario,
    MainScenario,
    MainScenarioQuest,
    SubScenario,
    GameCoinPoint,
    FriendPoint,
    BillingPoint,
    Familiarity,
    CardBackground,
    BraveCoin,
    GachaTicket,
    ClubActivityRequest,
    AdventureBook,
    StackEventPoint,
    BraveSystem,
    BundleAdventureBook,
    ConsumptionResourceItem,
    ExchangePoint,
    EvolutionItemSmall,
    EvolutionItemMiddle,
    EvolutionItemBig,
    ClubWorkingOrder
}

public enum DropBoxType
{
    None,
    Silver,
    Gold,
    Sancho = 10
}

public enum EnemySizeTye
{
    SMALL,
    MEDIUM,
    LARGE
}

public enum EnemyMoveType
{
    NORMAL,
    ACTIVE_MOVE_STOP,
    WAIT_AND_MOVE,
    HATE,
    HATE_ACTIVE_MOVE_STOP,
    HATE_WAIT_AND_MOVE,
    HATE_ACTIVESTOP_AND_WAITMOVE
}

public enum EnemyLineChangeMoveType
{
    NONE,
    BLANK,
    LOW_HP,
    RANDOM
}

public enum EnemyLineChangeTriggerType
{
    NONE,
    TIME,
    ATTACK
}

public enum EnemyHpGaugeType
{
    NONE,
    SHORT,
    NOMAL,
    BOSS
}
            
public enum AttributeType
{
    NONE,
    RED,
    BLUE,
    GREEN,
    YELLOW,
    PURPLE
}

public enum AttackType
{
    NONE,
    SHORT,
    MIDDLE,
    LONG,
    ASSIST
}
            
public enum FriendType
{
    OWNER = 1,
    FRIEND,
    GUEST
}