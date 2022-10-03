using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class EvolutionCardResultEntity : BaseEntity<EvolutionCardResultEntity>
    {
        public EvolutionCardResultEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            Request requestObj = Deserialize<Request>(requestBody)!;
            
            Card userCard = Card.Load(requestObj.id);
            DataModel.Card masterCard = userCard.MasterData();
            int targetEvolutionLevel = requestObj.card.evolution_level;
            int gotBraveCoin = masterCard.EvolutionRewardBraveCoin ?? 0;
            
            EvolutionRecipe recipe = DatabaseContexts.Cards.EvolutionRecipes.First(r => r.Id == masterCard.EvolutionRecipeId);

            // change stats of the card
            // default value will be used if it's a limit break
            long newMasterId = masterCard.EvolutionCardId ?? userCard.master_id;
            Utils.Log($"user card {userCard.id} master_id {userCard.master_id}->{newMasterId}");
            Utils.Log($"user card {userCard.id} evolution_level {userCard.evolution_level}->{targetEvolutionLevel}");
            userCard.master_id = newMasterId;
            userCard.evolution_level = targetEvolutionLevel;
            userCard.Save();
            
            // since the player cards key is base_card_id which won't change,
            // we don't do anything even if the master_id of the card changed
            DataModel.Card newMasterCard = userCard.MasterData();
            
            // consume player's items
            Dictionary<long, int> costResources = new Dictionary<long, int>(3)
            {
                {recipe.Resource1Id, recipe.Resource1Amount}
            };
            if (recipe.Resource2Id != null && recipe.Resource2Amount != null)
                costResources.Add(recipe.Resource2Id ?? 0, recipe.Resource2Amount ?? 0);
            if (recipe.Resource3Id != null && recipe.Resource3Amount != null)
                costResources.Add(recipe.Resource3Id ?? 0, recipe.Resource3Amount ?? 0);
            foreach (var resource in costResources)
            {
                // player has to have this item, or the client won't allow us to be here
                Item recipeItem = Item.Load(player.items.evolution[resource.Key]);
                // we don't need to validate for the same reason
                recipeItem.quantity -= resource.Value;
                recipeItem.Save();
                Utils.Log($"player item {recipeItem.master_id} quantity -{resource.Value}");
            }

            // consume the player's money
            player.data.money -= recipe.Cost; // don't need to clamp, client does that

            // grant the accessory rewards to the player
            long? evolutionRewardAccessoryId = newMasterCard.GetEvolutionRewardAccessoryId();
            if (evolutionRewardAccessoryId != null)
            {
                long accessoryId = evolutionRewardAccessoryId ?? 0;
                Accessory playerAccessory;
                if (!player.accessories.ContainsKey(accessoryId))
                {
                    playerAccessory = new()
                    {
                        id = Accessory.GetID(),
                        master_id = accessoryId,
                        level = 1,
                        quantity = 0
                    };
                    player.accessories.Add(accessoryId, playerAccessory.id);
                }
                else
                {
                    playerAccessory = Accessory.Load(player.accessories[accessoryId]);
                }

                int rewardQuantity = userCard.potential + 1;
                playerAccessory.quantity += rewardQuantity;
                playerAccessory.Save();
                
                Utils.Log($"reward accessory {accessoryId} quantity +{rewardQuantity}");
            }
            // grant the brave coin rewards to the player
            player.data.braveCoin += gotBraveCoin;
            Utils.Log($"player got {gotBraveCoin} brave coins");
            // TODO: <GIFT_SYSTEM> give the limit break gift to the player


            player.Save();

            Response responseObj = new()
            {
                card = CardsEntity.Card.FromPlayerCardData(userCard),
                brave_coin = gotBraveCoin
            };
 
            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Request
        {
            public CardEvolutionData card { get; set; } = new();
            public long id { get; set; } // user card id

            public class CardEvolutionData
            {
                public int evolution_level { get; set; }
            }
        }

        public class Response
        {
            public CardsEntity.Card card { get; set; } = new();
            public int brave_coin { get; set; }
        }
    }
}