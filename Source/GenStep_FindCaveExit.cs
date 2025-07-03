using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Shashlichnik
{
    public class GenStep_FindCaveExit : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 2162632;
            }
        }
        private IntVec3 FindCellForExit(Map map, bool shouldBeStandable)
        {
            IntVec3 result;
            if (shouldBeStandable)
            {
                if (CellFinder.TryFindRandomCell(map, (IntVec3 cell) => cell.Standable(map) && (float)cell.DistanceToEdge(map) > ClearRadius + 1f, out result))
                {
                    return result;
                }
                else
                {
                    return FindCellForExit(map, false);
                }
            }
            if (CellFinder.TryFindRandomCell(map, (IntVec3 cell) => cell.DistanceToEdge(map) > ClearRadius + 1f, out result) && result.InBounds(map))
            {
                return result;
            }
            Log.Warning("Can't find a valid place for a cave exit, placing at center");
            return map.Center;
        }
        public override void Generate(Map map, GenStepParams parms)
        {
            var shouldFindCave = Rand.Chance(0.5f);
            var exitCell = FindCellForExit(map, shouldFindCave);
            foreach (IntVec3 c in GenRadial.RadialCellsAround(exitCell, ClearRadius, true))
            {
                foreach (Thing thing in from t in c.GetThingList(map).ToList<Thing>()
                                        where t.def.destroyable
                                        select t)
                {
                    thing.Destroy(DestroyMode.Vanish);
                }
            }
            GenSpawn.Spawn(ThingMaker.MakeThing(DefsOf.ShashlichnikCaveExit, null), exitCell, map, WipeMode.Vanish);
            MapGenerator.PlayerStartSpot = exitCell;
        }

        public const float ClearRadius = 4.5f;
    }
}
