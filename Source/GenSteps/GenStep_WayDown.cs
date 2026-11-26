using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_WayDown : GenStep_CaveInterest
    {
        public GenStep_WayDown() : base()
        {
            countChances = new List<RimWorld.CountChance>
            {
                // Note: Get updated to be settings value on generation call
                new CountChance() {count = 1, chance = 1f},
            };
            subCountChances = new List<CountChance>()
            {
                new CountChance() {count = 1, chance = 1f }
            };
        }
        public override int SeedPart => 4536431;

        protected override bool TryGetNextInterestPosition(Map map, out IntVec3 result)
        {
            return CellFinder.TryFindRandomCell(map, c => c.DistanceToEdge(map) > 5 && !AllInterestCenters.Any((IntVec3 p) => c.InHorDistOf(p, MinDistApart)), out result);
        }
        protected override bool CellValidator(Map map, IntVec3 c) => !c.InHorDistOf(MapGenerator.PlayerStartSpot, distanceToPlayer) && c.DistanceToEdge(map) > 5 && !AllInterestCenters.Any((IntVec3 p) => c.InHorDistOf(p, MinDistApart));
        const float ClearRadius = 3f;

        public override void Generate(Map map, GenStepParams parms)
        {
            countChances[0] = countChances[0] with { chance = Mod.Settings.nestedCaveEntranceChance };
            base.Generate(map, parms);
        }

        protected override bool TrySpawnInterestAt(Map map, IntVec3 intVec)
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
            var caveEntrance = GenSpawn.Spawn(ThingMaker.MakeThing(DefsOf.ShashlichnikCaveEntrance, null), intVec, map, WipeMode.Vanish) as CaveEntrance;
            if (caveEntrance == null)
            {
                return false;
            }
            caveEntrance.TicksToOpen = (int)(caveEntrance.TicksToOpen * Rand.Range(0.3f, 0.6f));
            return true;
        }
    }
}
