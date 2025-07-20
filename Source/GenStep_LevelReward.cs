using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_LevelReward : GenStep
    {
        ThingSetMakerDef reward;
        IntRange countOfRewards;
        public override int SeedPart => 291812;
        public override void Generate(Map map, GenStepParams parms)
        {
            IEnumerable<Thing> rewards = Enumerable.Empty<Thing>();
            var count = countOfRewards.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                rewards = rewards.Union(reward.root.Generate());
            }
            foreach (var thing in rewards)
            {
                if (CellFinder.TryFindRandomCellNear(MapGenerator.PlayerStartSpot, map, 15, c => c.Standable(map), out var cell, 600))
                {
                    cell = CellFinder.FindNoWipeSpawnLocNear(cell, map, thing.def, Rot4.Random, 6);
                    GenSpawn.Spawn(thing, cell, map);
                }
                else
                {
                    Log.Warning($"Can't find a place for {thing}");
                }
            }
        }
    }
}
