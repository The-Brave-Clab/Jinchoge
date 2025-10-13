using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public class ChapterProgress : BasePlayerData<ChapterProgress, long>
{
    public long id { get; set; }
    public long master_id { get; set; } // from master_data
    public bool finished { get; set; }
    public IList<long> episodes { get; set; } = new List<long>(); // The episodes that the player viewed

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

    public static async Task<ChapterProgress> GetOrCreate(PlayerProfile player, long chapterId)
    {
        if (player.progress.chapters.TryGetValue(chapterId, out var id))
        {
            return await Load(id);
        }

        ChapterProgress progress = new()
        {
            id = await GetID(),
            master_id = chapterId,
            finished = false,
            episodes = new List<long>()
        };
        await progress.Save();
            
        player.progress.chapters.Add(chapterId, progress.id);
        await player.Save();

        return progress;
    }
}