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
    public class GenStep_SoilAndGravel : GenStep
    {
        private const float soilThreshold = 1.2f, gravelThreshold = 0.91f;
        public override int SeedPart => 92715;

        public override void Generate(Map map, GenStepParams parms)
        {
            var perlin = new Perlin(frequency: 0.0463, lacunarity: 0.71, persistence: 0.79, octaves: 2, 9203, QualityMode.Medium);
            foreach (var cell in map.AllCells)
            {
                var value = perlin.GetValue(cell);
                if (value < gravelThreshold)
                {
                    continue;
                }
                var edifice = cell.GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable)
                {                
                    edifice.Destroy(DestroyMode.Vanish);
                }

                if (value > soilThreshold)
                {
                    map.terrainGrid.SetTerrain(cell, TerrainDefOf.Soil);
                }
                else if (value >= gravelThreshold)
                {
                    map.terrainGrid.SetTerrain(cell, TerrainDefOf.Gravel);
                }
            }
        }
    }
}
