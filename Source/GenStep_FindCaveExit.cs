using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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

        public override void Generate(Map map, GenStepParams parms)
        {
            var shouldFindCave = Rand.Chance(0.5f);
            CellFinder.TryFindRandomCell(map, (IntVec3 cell) => (!shouldFindCave  || cell.Standable(map)) && (float)cell.DistanceToEdge(map) > ClearRadius + 1f, out var intVec);
            foreach (IntVec3 c in GenRadial.RadialCellsAround(intVec, ClearRadius, true))
            {
                foreach (Thing thing in from t in c.GetThingList(map).ToList<Thing>()
                                        where t.def.destroyable
                                        select t)
                {
                    thing.Destroy(DestroyMode.Vanish);
                }
            }
            GenSpawn.Spawn(ThingMaker.MakeThing(DefsOf.CaveExit, null), intVec, map, WipeMode.Vanish);
            MapGenerator.PlayerStartSpot = intVec;
        }

        public const float ClearRadius = 4.5f;
    }
}
