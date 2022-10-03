using System.Collections.Generic;

namespace Yuyuyui.PrivateServer
{
    public class ChapterProgress : BasePlayerData<ChapterProgress, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public bool finished { get; set; }
        public IList<long> episodes { get; set; } = new List<long>(); // The episodes that the player viewed

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

        public static ChapterProgress GetOrCreate(PlayerProfile player, long chapterId)
        {
            if (player.progress.chapters.ContainsKey(chapterId))
            {
                long id = player.progress.chapters[chapterId];
                return Load(id);
            }

            ChapterProgress progress = new()
            {
                id = GetID(),
                master_id = chapterId,
                finished = false,
                episodes = new List<long>()
            };
            progress.Save();
            
            player.progress.chapters.Add(chapterId, progress.id);
            player.Save();

            return progress;
        }
    }
}