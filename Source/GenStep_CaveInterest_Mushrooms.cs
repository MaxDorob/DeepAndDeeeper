using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_Mushrooms : GenStep_CaveInterest
    {
        List<ThingDef> possibleDefs;
        public IntRange PatchSizeRange = new IntRange(50, 70);
        public float PatchDensity = 0.7f;

        public override int SeedPart => 727162;

        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            possibleDefs ??= new List<ThingDef>
            {
                ThingDefOf.Plant_HealrootWild,
                ThingDefOf.Glowstool,
                ThingDefOf.Bryolux,
                ThingDefOf.Agarilux
            };

            foreach (IntVec3 intVec in GridShapeMaker.IrregularLump(thingPos, map, PatchSizeRange.RandomInRange))
            {
                map.terrainGrid.SetTerrain(intVec, TerrainDefOf.SoilRich);
                if (intVec.GetPlant(map) == null && intVec.GetCover(map) == null && intVec.GetEdifice(map) == null && Rand.Chance(PatchDensity))
                {
                    ((Plant)GenSpawn.Spawn(ThingMaker.MakeThing(possibleDefs.RandomElement(), null), intVec, map, WipeMode.Vanish)).Growth = Mathf.Clamp01(WildPlantSpawner.InitialGrowthRandomRange.RandomInRange);
                }
            }
            return true;
        }
    }
}
