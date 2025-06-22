using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_ExitToSurfaceInterest : GenStep_FindCaveExit
    {
        public override int SeedPart => 424682;
        public float chance = 1f;
        public override void Generate(Map map, GenStepParams parms)
        {

            if (Rand.Chance(chance) && CellFinder.TryFindRandomCell(map, (IntVec3 cell) => MapGenerator.PlayerStartSpot.DistanceTo(cell) > 40f && (float)cell.DistanceToEdge(map) > ClearRadius + 1f, out var intVec))
            {
                foreach (IntVec3 c in GenRadial.RadialCellsAround(intVec, ClearRadius, true))
                {
                    foreach (Thing thing in from t in c.GetThingList(map).ToList<Thing>()
                                            where t.def.destroyable
                                            select t)
                    {
                        thing.Destroy(DestroyMode.Vanish);
                    }
                }
                GenSpawn.Spawn(ThingMaker.MakeThing(DefsOf.CaveExitToSurfaceInterest, null), intVec, map, WipeMode.Vanish);
            }
        }
    }
}
