using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class StageProgress : BasePlayerData<StageProgress, long>
{
    public long id { get; set; }
    public long master_id { get; set; } // from master_data
    public bool finished { get; set; }
    public bool finishedInTime { get; set; }
    public bool finishedNoInjury { get; set; }

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
        
    public static async Task<StageProgress> GetOrCreate(PlayerProfile player, long stageId)
    {
        if (player.progress.stages.TryGetValue(stageId, out var id))
        {
            return await Load(id);
        }

        StageProgress progress = new()
        {
            id = await GetID(),
            master_id = stageId,
            finished = false,
            finishedInTime = false,
            finishedNoInjury = false
        };
        await progress.Save();
            
        player.progress.stages.Add(stageId, progress.id);
        await player.Save();

        return progress;
    }
}