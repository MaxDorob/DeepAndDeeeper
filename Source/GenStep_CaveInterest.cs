using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
namespace Shashlichnik
{
    public abstract class GenStep_CaveInterest : GenStep
    {
        public List<IntVec3> AllInterestCenters
        {
            get
            {
                if (!MapGenerator.TryGetVar<List<IntVec3>>("ShashlichnikCaveInterestCells", out var list))
                {
                    list = new List<IntVec3>();
                    MapGenerator.SetVar("ShashlichnikCaveInterestCells", list);
                }
                return list;
            }
        }
        protected virtual bool TryGetNextInterestPosition(Map map, out IntVec3 result)
        {
            return CellFinder.TryFindRandomCell(map, c => c.Standable(map) && c.DistanceToEdge(map) > 5 && !AllInterestCenters.Any((IntVec3 p) => c.InHorDistOf(p, MinDistApart)), out result);
        }
        protected bool TryGenerateInterestAt(Map map, IntVec3 center)
        {
            int count = SubCount;
            foreach (IntVec3 cell in GridShapeMaker.IrregularLump(center, map, InterestPointSize, null).Take(100))
            {
                if (TrySpawnInterestAt(map, cell))
                {
                    if (--count <= 0)
                    {
                        return true;
                    }
                }
            }
            Log.Error($"{this.GetType().Name} can't generate interest");
            return false;
        }
        public List<CountChance> countChances = new List<CountChance>()
        {
            new CountChance() {count = 0, chance = 0.625f},
            new CountChance() {count = 1, chance = 0.25f},
            new CountChance() {count = 2, chance = 0.125f},
        };
        protected virtual int Count => CountChanceUtility.RandomCount(countChances);
        protected virtual int SubCount => CountChanceUtility.RandomCount(countChances);
        public List<CountChance> subCountChances = new List<CountChance>()
        {
            new CountChance() {count = 1, chance = 0.25f},
            new CountChance() {count = 2, chance = 0.25f + 0.125f},
            new CountChance() {count = 3, chance = 0.125f},
            new CountChance() {count = 4, chance = 0.125f},
        };
        protected abstract bool TrySpawnInterestAt(Map map, IntVec3 thingPos);

        protected virtual bool CellValidator(Map map, IntVec3 c) => c.Standable(map) && !c.InHorDistOf(MapGenerator.PlayerStartSpot, 5f) && c.DistanceToEdge(map) > 5 && !AllInterestCenters.Any((IntVec3 p) => c.InHorDistOf(p, MinDistApart));
        public override void Generate(Map map, GenStepParams parms)
        {
            int randomInRange = Count;

            List<IntVec3> interestPoints = new List<IntVec3>();
            for (int i = 0; i < randomInRange; i++)
            {
                if (CellFinderLoose.TryGetRandomCellWith(c => CellValidator(map, c), map, 100, out var cell))
                {
                    interestPoints.Add(cell);
                }
            }
            foreach (IntVec3 intVec in interestPoints)
            {
                if (TryGenerateInterestAt(map, intVec))
                {
                    AllInterestCenters.Add(intVec);
                }
            }
        }

        public int InterestPointSize = 20;
        public float MinDistApart = 10f;
        public IntRange InterestPointCountRange = new IntRange(0, 3);
     
    }
}
