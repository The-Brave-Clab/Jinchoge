using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public class EpisodeProgress : BasePlayerData<EpisodeProgress, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public bool finished { get; set; }
        public IList<long> stages { get; set; } = new List<long>(); // The stages that the player viewed

        public override long Identifier => id;
        
        private static async Task<long> GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (await Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }
        
        public static async Task<EpisodeProgress> GetOrCreate(PlayerProfile player, long episodeId)
        {
            if (player.progress.episodes.TryGetValue(episodeId, out var id))
            {
                return await Load(id);
            }

            EpisodeProgress progress = new()
            {
                id = await GetID(),
                master_id = episodeId,
                finished = false,
                stages = new List<long>()
            };
            await progress.Save();
            
            player.progress.episodes.Add(episodeId, progress.id);
            await player.Save();

            return progress;
        }
    }
}