namespace Yuyuyui.PrivateServer
{
    public class StageProgress : BasePlayerData<StageProgress, long>
    {
        public long id { get; set; }
        public long master_id { get; set; } // from master_data
        public bool finished { get; set; }
        public bool finishedInTime { get; set; }
        public bool finishedNoInjury { get; set; }

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
        
        public static StageProgress GetOrCreate(PlayerProfile player, long stageId)
        {
            if (player.progress.stages.ContainsKey(stageId))
            {
                long id = player.progress.stages[stageId];
                return Load(id);
            }

            StageProgress progress = new()
            {
                id = GetID(),
                master_id = stageId,
                finished = false,
                finishedInTime = false,
                finishedNoInjury = false
            };
            progress.Save();
            
            player.progress.stages.Add(stageId, progress.id);
            player.Save();

            return progress;
        }
    }
}