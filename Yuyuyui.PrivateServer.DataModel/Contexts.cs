namespace Yuyuyui.PrivateServer.DataModel;

public class Contexts : IDisposable
{
    private static Contexts? _instance = null;

    private AccessoriesContext           _accessoriesContext;
    private ActiveSkillsContext          _activeSkillsContext;
    private ActivityRequestSheetsContext _activityRequestSheetsContext;
    private AdventureBooksContext        _adventureBooksContext;
    private BraveSystemContext           _braveSystemContext;
    private CardsContext                 _cardsContext;
    private CartoonsContext              _cartoonsContext;
    private CharactersContext            _charactersContext;
    private ClubWorkingsContext          _clubWorkingsContext;
    private EnemiesContext               _enemiesContext;
    private EnhancementContext           _enhancementContext;
    private EventStoriesContext          _eventStoriesContext;
    private GachasContext                _gachasContext;
    private GiftsContext                 _giftsContext;
    private ItemsContext                 _itemsContext;
    private LoginBonusContext            _loginBonusContext;
    private QuestsContext                _questsContext;
    private SkillsContext                _skillsContext;
    private StackPointEventContext       _stackPointEventContext;
    private UserLevelsContext            _userLevelsContext;
    
    public AccessoriesContext           Accessories           => _instance!._accessoriesContext;
    public ActiveSkillsContext          ActiveSkills          => _instance!._activeSkillsContext;
    public ActivityRequestSheetsContext ActivityRequestSheets => _instance!._activityRequestSheetsContext;
    public AdventureBooksContext        AdventureBooks        => _instance!._adventureBooksContext;
    public BraveSystemContext           BraveSystem           => _instance!._braveSystemContext;
    public CardsContext                 Cards                 => _instance!._cardsContext;
    public CartoonsContext              Cartoons              => _instance!._cartoonsContext;
    public CharactersContext            Characters            => _instance!._charactersContext;
    public ClubWorkingsContext          ClubWorkings          => _instance!._clubWorkingsContext;
    public EnemiesContext               Enemies               => _instance!._enemiesContext;
    public EnhancementContext           Enhancement           => _instance!._enhancementContext;
    public EventStoriesContext          EventStories          => _instance!._eventStoriesContext;
    public GachasContext                Gachas                => _instance!._gachasContext;
    public GiftsContext                 Gifts                 => _instance!._giftsContext;
    public ItemsContext                 Items                 => _instance!._itemsContext;
    public LoginBonusContext            LoginBonus            => _instance!._loginBonusContext;
    public QuestsContext                Quests                => _instance!._questsContext;
    public SkillsContext                Skills                => _instance!._skillsContext;
    public StackPointEventContext       StackPointEvent       => _instance!._stackPointEventContext;
    public UserLevelsContext            UserLevels            => _instance!._userLevelsContext;

    private Contexts()
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
        _instance = new Contexts();
    }

    public static void Destroy()
    {
        if (_instance == null) return;
        _instance.Dispose();
        _instance = null;
    }
}