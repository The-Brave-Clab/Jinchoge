namespace Yuyuyui.PrivateServer.DataModel;

public class DatabaseContexts : IDisposable
{
    private static DatabaseContexts? _instance = null;

    private readonly AccessoriesContext           _accessoriesContext;
    private readonly ActiveSkillsContext          _activeSkillsContext;
    private readonly ActivityRequestSheetsContext _activityRequestSheetsContext;
    private readonly AdventureBooksContext        _adventureBooksContext;
    private readonly BraveSystemContext           _braveSystemContext;
    private readonly CardsContext                 _cardsContext;
    private readonly CartoonsContext              _cartoonsContext;
    private readonly CharactersContext            _charactersContext;
    private readonly ClubWorkingsContext          _clubWorkingsContext;
    private readonly EnemiesContext               _enemiesContext;
    private readonly EnhancementContext           _enhancementContext;
    private readonly EventStoriesContext          _eventStoriesContext;
    private readonly GachasContext                _gachasContext;
    private readonly GiftsContext                 _giftsContext;
    private readonly ItemsContext                 _itemsContext;
    private readonly LoginBonusContext            _loginBonusContext;
    private readonly QuestsContext                _questsContext;
    private readonly SkillsContext                _skillsContext;
    private readonly StackPointEventContext       _stackPointEventContext;
    private readonly UserLevelsContext            _userLevelsContext;

    public static AccessoriesContext           Accessories           => _instance!._accessoriesContext;
    public static ActiveSkillsContext          ActiveSkills          => _instance!._activeSkillsContext;
    public static ActivityRequestSheetsContext ActivityRequestSheets => _instance!._activityRequestSheetsContext;
    public static AdventureBooksContext        AdventureBooks        => _instance!._adventureBooksContext;
    public static BraveSystemContext           BraveSystem           => _instance!._braveSystemContext;
    public static CardsContext                 Cards                 => _instance!._cardsContext;
    public static CartoonsContext              Cartoons              => _instance!._cartoonsContext;
    public static CharactersContext            Characters            => _instance!._charactersContext;
    public static ClubWorkingsContext          ClubWorkings          => _instance!._clubWorkingsContext;
    public static EnemiesContext               Enemies               => _instance!._enemiesContext;
    public static EnhancementContext           Enhancement           => _instance!._enhancementContext;
    public static EventStoriesContext          EventStories          => _instance!._eventStoriesContext;
    public static GachasContext                Gachas                => _instance!._gachasContext;
    public static GiftsContext                 Gifts                 => _instance!._giftsContext;
    public static ItemsContext                 Items                 => _instance!._itemsContext;
    public static LoginBonusContext            LoginBonus            => _instance!._loginBonusContext;
    public static QuestsContext                Quests                => _instance!._questsContext;
    public static SkillsContext                Skills                => _instance!._skillsContext;
    public static StackPointEventContext       StackPointEvent       => _instance!._stackPointEventContext;
    public static UserLevelsContext            UserLevels            => _instance!._userLevelsContext;

    private DatabaseContexts()
    {
        _accessoriesContext = new AccessoriesContext();
        _activeSkillsContext = new ActiveSkillsContext();
        _activityRequestSheetsContext = new ActivityRequestSheetsContext();
        _adventureBooksContext = new AdventureBooksContext();
        _braveSystemContext = new BraveSystemContext();
        _cardsContext = new CardsContext();
        _cartoonsContext = new CartoonsContext();
        _charactersContext = new CharactersContext();
        _clubWorkingsContext = new ClubWorkingsContext();
        _enemiesContext = new EnemiesContext();
        _enhancementContext = new EnhancementContext();
        _eventStoriesContext = new EventStoriesContext();
        _gachasContext = new GachasContext();
        _giftsContext = new GiftsContext();
        _itemsContext = new ItemsContext();
        _loginBonusContext = new LoginBonusContext();
        _questsContext = new QuestsContext();
        _skillsContext = new SkillsContext();
        _stackPointEventContext = new StackPointEventContext();
        _userLevelsContext = new UserLevelsContext();
    }

    public void Dispose()
    {
        _accessoriesContext.Dispose();
        _activeSkillsContext.Dispose();
        _activityRequestSheetsContext.Dispose();
        _adventureBooksContext.Dispose();
        _braveSystemContext.Dispose();
        _cardsContext.Dispose();
        _cartoonsContext.Dispose();
        _charactersContext.Dispose();
        _clubWorkingsContext.Dispose();
        _enemiesContext.Dispose();
        _enhancementContext.Dispose();
        _eventStoriesContext.Dispose();
        _gachasContext.Dispose();
        _giftsContext.Dispose();
        _itemsContext.Dispose();
        _loginBonusContext.Dispose();
        _questsContext.Dispose();
        _skillsContext.Dispose();
        _stackPointEventContext.Dispose();
        _userLevelsContext.Dispose();
    }

    public static void Initialize()
    {
        _instance = new DatabaseContexts();
    }

    public static void Destroy()
    {
        if (_instance == null) return;
        _instance.Dispose();
        _instance = null;
    }
}