using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer.Strategy;

public class ObtainableCardTitleDeterminationStrategy
{
    private const int MINIMAL_CARD_POTENTIAL = 1;
    private const int MINIMAL_CARD_LEVEL = 99;
    private const int MINIMAL_EVOLUTION_LEVEL = 5;
    private const int CARD_TITLE_CONTENT_TYPE = 2;
    private static List<int> ELIGIBLE_RARITY_LIST = new List<int> { 400, 450, 500 };
    
    public static IQueryable<TitleItem> Determine(PlayerProfile playerProfile, CardsContext cardsContext, ItemsContext itemsContext)
    {
        var userEligibleCardIdList = GetUserBaseCardIdListEligibleForObtainingTitle(playerProfile, cardsContext);
        return GetUserObtainableTitleItems(playerProfile, itemsContext, userEligibleCardIdList);
    }

    private static IQueryable<TitleItem> GetUserObtainableTitleItems(PlayerProfile playerProfile, ItemsContext itemsContext, IEnumerable<long> userEligibleCardIdList)
    {
        return itemsContext.TitleItems
            .Where(titleItem => !playerProfile.items.titleItems.Contains(titleItem.Id))
            .Where(titleItem => titleItem.ContentType == CARD_TITLE_CONTENT_TYPE && titleItem.Priority != null)
            .Where(titleItem => userEligibleCardIdList.Contains(titleItem.Priority.GetValueOrDefault(0)));
    }

    private static IEnumerable<long> GetUserBaseCardIdListEligibleForObtainingTitle(PlayerProfile playerProfile, CardsContext cardsContext)
    {
        return playerProfile.cards.Values
            .Select(value => Card.Load(value))
            .Where(card => ELIGIBLE_RARITY_LIST.Contains(card.MasterData(cardsContext).Rarity))
            .Where(card => card.potential >= MINIMAL_CARD_POTENTIAL && card.level >= MINIMAL_CARD_LEVEL)
            .Where(card => card.evolution_level >= MINIMAL_EVOLUTION_LEVEL)
            .Select(card => card.MasterData(cardsContext).BaseCardId);
    }
}