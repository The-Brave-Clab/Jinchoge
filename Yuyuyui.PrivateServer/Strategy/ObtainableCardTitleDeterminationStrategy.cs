using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer.Strategy;

public class ObtainableCardTitleDeterminationStrategy
{
    private const int MINIMAL_CARD_POTENTIAL = 1;
    private const int MINIMAL_CARD_LEVEL = 99;
    private const int MINIMAL_EVOLUTION_LEVEL = 5;
    private const int CARD_TITLE_CONTENT_TYPE = 2;
    private List<int> ELIGIBLE_RARITY_LIST = new List<int> { 400, 450, 500 };
    private readonly ItemsContext _itemContext = new ();
    private readonly CardsContext _cardsContext = new ();

    public ObtainableCardTitleDeterminationStrategy()
    {
        
    }

    public IQueryable<TitleItem> Determine(PlayerProfile playerProfile)
    {
        var userEligibleCardIdList = GetUserBaseCardIdListEligibleForObtainingTitle(playerProfile);
        return GetUserObtainableTitleItems(playerProfile, userEligibleCardIdList);
        
    }

    private IQueryable<TitleItem> GetUserObtainableTitleItems(PlayerProfile playerProfile, IEnumerable<long> userEligibleCardIdList)
    {
        return _itemContext.TitleItems
            .Where(titleItem => !playerProfile.items.titleItems.Contains(titleItem.Id))
            .Where(titleItem => titleItem.ContentType == CARD_TITLE_CONTENT_TYPE && titleItem.Priority != null)
            .Where(titleItem => userEligibleCardIdList.Contains(titleItem.Priority.GetValueOrDefault(0)));
    }

    private IEnumerable<long> GetUserBaseCardIdListEligibleForObtainingTitle(PlayerProfile playerProfile)
    {
        return playerProfile.cards.Values
            .Select(value => Card.Load(value))
            .Where(card => ELIGIBLE_RARITY_LIST.Contains(card.MasterData(_cardsContext).Rarity))
            .Where(card => card.potential >= MINIMAL_CARD_POTENTIAL && card.level >= MINIMAL_CARD_LEVEL)
            .Where(card => card.evolution_level >= MINIMAL_EVOLUTION_LEVEL)
            .Select(card => card.MasterData(_cardsContext).BaseCardId);
    }
}