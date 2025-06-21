using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_Chemfuel : GenStep_CaveInterest
    {
        protected override bool TrySpawnInterestAt(Map map, IntVec3 cell)
        {
            GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.AncientGenerator ?? ThingDefOf.ChemfuelPoweredGenerator, null), cell, map, WipeMode.Vanish);
            int randomInRange = ChemfuelCountRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                if (CellFinder.TryFindRandomCellNear(cell, map, ChemfuelStackMaxDist, c => c.Standable(map), out var loc))
                {
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Chemfuel, null);
                    thing.stackCount = ChemfuelStackCountRange.RandomInRange;
                    GenSpawn.Spawn(thing, loc, map, WipeMode.Vanish);
                }
                
            }
            foreach (IntVec3 c2 in GridShapeMaker.IrregularLump(cell, map, ChemfuelPuddleSize, null))
            {
                if (c2.GetEdifice(map) == null)
                {
                    FilthMaker.TryMakeFilth(c2, map, ThingDefOf.Filth_Fuel, 1, FilthSourceFlags.None, true);
                }
            }
            return true;
        }
        public IntRange ChemfuelCountRange = new IntRange(3, 5);
        public IntRange ChemfuelStackCountRange = new IntRange(10, 20);
        public int ChemfuelPuddleSize = 20;
        public int ChemfuelStackMaxDist = 2;

        public override int SeedPart => 3286528;
    }
}
