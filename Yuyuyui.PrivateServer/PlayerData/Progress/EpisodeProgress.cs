namespace Yuyuyui.PrivateServer
{
    public class EpisodeProgress : BasePlayerData<EpisodeProgress, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public bool finished { get; set; }
        public IList<long> stages { get; set; } = new List<long>(); // The stages that the player viewed

        protected override long Identifier => id;
        
        private static long GetID()
        {
            long new_id = long.Parse(Utils.GenerateRandomDigit(9));
            while (Exists(new_id))
            {
                new_id = long.Parse(Utils.GenerateRandomDigit(9));
            }

            return new_id;
        }
        
        public static EpisodeProgress GetOrCreate(PlayerProfile player, long episodeId)
        {
            if (player.progress.episodes.ContainsKey(episodeId))
            {
                long id = player.progress.episodes[episodeId];
                return Load(id);
            }

            EpisodeProgress progress = new()
            {
                id = GetID(),
                master_id = episodeId,
                finished = false,
                stages = new List<long>()
            };
            progress.Save();
            
            player.progress.episodes.Add(episodeId, progress.id);
            player.Save();

            return progress;
        }
    }
}