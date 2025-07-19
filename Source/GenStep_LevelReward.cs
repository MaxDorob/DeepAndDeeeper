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
        public override int SeedPart => 291812;
        public override void Generate(Map map, GenStepParams parms)
        {
            var rewards = reward.root.Generate();
            foreach (var thing in rewards)
            {
                if (CellFinder.TryFindRandomCellNear(MapGenerator.PlayerStartSpot, map, 15, c => c.Standable(map) && GenSpawn.CanSpawnAt(thing.def, c, map, canWipeEdifices: false), out var cell, 600))
                {
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
