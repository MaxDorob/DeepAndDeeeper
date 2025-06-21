using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using UnityEngine.Networking.Types;

namespace Shashlichnik
{
    internal class GenStep_CaveInterest_CorpseGear : GenStep_CaveInterest_CorpsePile
    {

        List<ThingDef> possibleDefs;
        public IntRange GearStackCountRange = new IntRange(1, 3);
        public int GearDist = 1;
        protected override void Spawn(Corpse corpse, Map map, IntVec3 corpseCell)
        {
            possibleDefs ??= new List<ThingDef>
            {
                ThingDefOf.MedicineIndustrial,
                ThingDefOf.MealSurvivalPack
            };
            base.Spawn(corpse, map, corpseCell);
            if (CellFinder.TryFindRandomCellNear(corpseCell, map, GearDist, (IntVec3 c) => c.Standable(map), out var loc))
            {
                Thing thing = ThingMaker.MakeThing(possibleDefs.RandomElement<ThingDef>(), null);
                thing.stackCount = GearStackCountRange.RandomInRange;
                GenSpawn.Spawn(thing, loc, map, WipeMode.Vanish);
            }
        }

    }
}
