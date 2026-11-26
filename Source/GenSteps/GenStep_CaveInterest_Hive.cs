using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_Hive : GenStep_CaveInterest
    {
        protected override bool TrySpawnInterestAt(Map map, IntVec3 cell)
        {
            (GenSpawn.Spawn(ThingDefOf.Hive, cell, map, WipeMode.Vanish) as Hive).GetComp<CompSpawnerHives>().canSpawnHives = false;
            int randomInRange = JellyCountRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                if (CellFinder.TryFindRandomCellNear(cell, map, SpawnRadius, c => c.Standable(map), out var loc))
                {
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.InsectJelly, null);
                    thing.stackCount = JellyStackCountRange.RandomInRange;
                    thing.SetForbidden(true, true);
                    GenSpawn.Spawn(thing, loc, map, WipeMode.Vanish);
                }
            }
            int randomInRange2 = GlowPodCountRange.RandomInRange;
            for (int j = 0; j < randomInRange2; j++)
            {
                if (CellFinder.TryFindRandomCellNear(cell, map, SpawnRadius, c => c.Standable(map), out var loc2))
                {
                    GenSpawn.Spawn(ThingDefOf.GlowPod, loc2, map, WipeMode.Vanish);
                }
            }
            foreach (IntVec3 filthCell in GridShapeMaker.IrregularLump(cell, map, FilthAreaSize, null))
            {
                if (filthCell.GetEdifice(map) == null && Rand.Chance(FilthSpawnChance))
                {
                    FilthMaker.TryMakeFilth(filthCell, map, ThingDefOf.Filth_Slime, 1, FilthSourceFlags.None, true);
                }
            }
            return true;
        }
        public int SpawnRadius = 3;
        public int FilthAreaSize = 20;
        public float FilthSpawnChance = 0.3f;
        public IntRange JellyCountRange = new IntRange(2, 3);
        public IntRange JellyStackCountRange = new IntRange(15, 40);
        public IntRange GlowPodCountRange = new IntRange(1, 2);

        public override int SeedPart => 263521;
    }
}
