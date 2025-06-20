using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveEntrance : GenStep
    {
        public override int SeedPart => 8423627;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (CellFinderLoose.TryGetRandomCellWith(c => c.DistanceToEdge(map) < 25 && c.Standable(map) && GenConstruct.CanPlaceBlueprintAt(DefsOf.CaveEntrance, c, Rot4.North, map, false), map, 150, out var result))
            {
                var cave = GenSpawn.Spawn(DefsOf.CaveEntrance, result, map) as CaveEntrance;
                cave.TicksToOpen = (int)(cave.TicksToOpen * Rand.Range(0.1f, 0.7f));
            }
            else
            {
                Log.Error("Can't find a place for cave entrance");
            }
        }
    }
}
