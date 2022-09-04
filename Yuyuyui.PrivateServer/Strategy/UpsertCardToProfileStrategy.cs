using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer.Strategy;

public static class UpsertCardToProfileStrategy
{
    private const int SP_INCREMENT = 2;
    private const int SUPPORT_SKILL_LEVEL_INCREMENT = 1;

    public static void Handle(PlayerProfile player, long masterCardId,
        int potentialCount, CardsContext cardsDb, GiftsContext giftsDb, ItemsContext itemsDb)
    {
        bool isNewCard = !player.cards.Keys.Contains(masterCardId);

        if (isNewCard)
        {
            var newCard = Card.NewCardByMasterId(masterCardId);
            player.cards.Add(masterCardId, newCard.id);
            newCard.Save();
            player.Save();
            potentialCount -= 1;
            Utils.Log("Assigned new card masterId=" + masterCardId + " to player.");
        }

        var playerCard = Card.Load(player.cards[masterCardId]);
        int previousPotentialCount = Card.Load(player.cards[masterCardId]).potential;
        HandlePlayerCardPotential(playerCard, potentialCount, player);

        DataModel.Card masterCard = cardsDb.Cards.First(card => card.Id == masterCardId);
        UpsertPotentialGift(previousPotentialCount, masterCard, playerCard, giftsDb, player);

        playerCard = Card.Load(player.cards[masterCardId]);
        var playerCardEvolutionLevel = playerCard.evolution_level;
        UpdateEvolutionAccessoriesForCard(playerCardEvolutionLevel, potentialCount, cardsDb, playerCard, player);

        AddEligibleCardTitleToPlayerProfile(player, cardsDb, itemsDb);
    }

    private static void AddEligibleCardTitleToPlayerProfile(PlayerProfile player, CardsContext cardsContext, ItemsContext itemsContext)
    {
        IQueryable<TitleItem> eligibleCardTitleItems = ObtainableCardTitleDeterminationStrategy.Determine(player, cardsContext, itemsContext);
        eligibleCardTitleItems.ForEach(titleItem => player.items.titleItems.Add(titleItem.Id));
        player.Save();
    }

    private static void UpdateEvolutionAccessoriesForCard(int playerCardEvolutionLevel, int potentialCount, CardsContext cardsDb,
        Card playerCard, PlayerProfile player)
    {
        if (playerCardEvolutionLevel < 1)
        {
            return;
        }

        if (potentialCount < 1)
        {
            return;
        }
        
        cardsDb.Cards
            .Where(card => card.Id == playerCard.master_id)
            .ForEach(card => UpdateEvolutionAccessories(potentialCount, player, card));

        player.Save();
    }

    private static void UpdateEvolutionAccessories(int potentialCount, PlayerProfile player, DataModel.Card card)
    {
        if (card.EvolutionRewardAccessory1Id != null)
        {
            UpdateEvolutionAccessory(player, card.EvolutionRewardAccessory1Id.Value, potentialCount);
        }

        if (card.EvolutionRewardAccessory2Id != null)
        {
            UpdateEvolutionAccessory(player, card.EvolutionRewardAccessory2Id.Value, potentialCount);
        }

        if (card.EvolutionRewardAccessory3Id != null)
        {
            UpdateEvolutionAccessory(player, card.EvolutionRewardAccessory3Id.Value, potentialCount);
        }

        if (card.EvolutionRewardAccessory4Id != null)
        {
            UpdateEvolutionAccessory(player, card.EvolutionRewardAccessory4Id.Value, potentialCount);
        }

        if (card.EvolutionRewardAccessory5Id != null)
        {
            UpdateEvolutionAccessory(player, card.EvolutionRewardAccessory5Id.Value, potentialCount);
        }
    }

    private static void UpdateEvolutionAccessory(PlayerProfile player, long accessoryId, int potentialCount)
    {
        var evolutionAccessory = Accessory.Load(player.accessories[accessoryId]);
        evolutionAccessory.quantity += potentialCount;
        evolutionAccessory.Save();
    }

    private static void UpsertPotentialGift(int previousPotentialCount, DataModel.Card masterCard, Card playerCard,
        GiftsContext giftsDb, PlayerProfile player)
    {
        if (previousPotentialCount >= masterCard.PotentialGiftBorder)
        {
            return;
        }

        if (playerCard.potential < masterCard.PotentialGiftBorder)
        {
            return;
        }
        
        long? potentialGiftId = masterCard.PotentialGiftId;
        DataModel.Gift masterGift = giftsDb.Gifts
            .Where(gift => gift.ContentType == "Accessory")
            .First(gift => gift.Id == potentialGiftId);

        bool isNewPotentialGift = !player.accessories.Keys.Contains(masterGift.ContentId);

        if (isNewPotentialGift)
        {
            var newAccessory = Accessory.NewAccessoryByMasterId(masterGift.ContentId);
            player.accessories.Add(masterGift.ContentId, newAccessory.id);
            newAccessory.Save();
            player.Save();
            Utils.Log("Assigned new Accessory masterId=" + masterGift.Id + " to player.");
            return;
        }
        
        var userAccessory = Accessory.Load(player.accessories[masterGift.ContentId]);
        userAccessory.quantity += 1;
        userAccessory.Save();
        player.Save();
        Utils.Log("Accessory Qty increased");
    }

    private static void HandlePlayerCardPotential(Card playerCard, int potentialCount, PlayerProfile player)
    {
        playerCard.potential += potentialCount;
        for (var i = 0; i < potentialCount; i++)
        {
            if (playerCard.support_skill_level < 20)
            {
                playerCard.support_skill_level += SUPPORT_SKILL_LEVEL_INCREMENT;
            }
            
            Random rand = new Random();
            if (rand.NextDouble() >= 0.5)
            {
                playerCard.base_sp_increment += SP_INCREMENT;
            }
        }

        playerCard.Save();
        player.Save();
        Utils.Log("Potential Increment Done.");
    }
}