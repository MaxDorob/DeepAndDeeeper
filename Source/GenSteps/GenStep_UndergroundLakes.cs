using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_UndergroundLakes : GenStep
    {
        public override int SeedPart => 8246281;

        public override void Generate(Map map, GenStepParams parms)
        {
            var count = Rand.Range(0, 3);
            for (int i = 0; i < count; i++)
            {
                GenerateLake(map, CellFinder.RandomCell(map));
            }
        }
        private void GenerateLake(Map map, IntVec3 center)
        {
            var lakeCells = GenRadial.RadialCellsAround(center, Rand.Range(1, 4), true);
            var subLakesCount = Rand.Range(0, 5);
            for (var i = 0; i < subLakesCount; i++)
            {
                var subLakeCells = GenRadial.RadialCellsAround(lakeCells.RandomElement(), Rand.Range(1, 4), false);
                lakeCells = lakeCells.Union(subLakeCells);
            }
            lakeCells = lakeCells.Distinct().Where(x => x.InBounds(map));
            foreach (var cell in lakeCells)
            {
                cell.GetEdifice(map)?.Destroy();
                map.terrainGrid.SetTerrain(cell, TerrainDefOf.WaterShallow);
            }
        }
    }
}
