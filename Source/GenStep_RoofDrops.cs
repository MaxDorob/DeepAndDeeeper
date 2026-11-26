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
    public class GenStep_RoofDrops : GenStep
    {
        public float dropThreshold = 0.4f;
        public float chanceModifier = 0.3f;
        public override int SeedPart => 2926527;
        public float frequency = 0.1961f;
        public float lacunarity = 0.47f;
        public float persistence = 0.37f;
        public int octaves = 4;

        public IntRange? inDistFromPlayerSpot;

        public override void Generate(Map map, GenStepParams parms)
        {
            var perlin = new Perlin(frequency: this.frequency, lacunarity: this.lacunarity, persistence: this.persistence, octaves: this.octaves, 203, QualityMode.Medium);
            foreach (var cell in map.AllCells)
            {
                if (inDistFromPlayerSpot != null)
                {
                    var dist = cell.DistanceTo(MapGenerator.PlayerStartSpot);
                    if (!inDistFromPlayerSpot.Value.Includes((int)dist))
                    {
                        continue;
                    }
                }
                var value = perlin.GetValue(cell);

                var edifice = cell.GetEdifice(map);
                if (edifice != null)
                {
                    continue;
                }


                if (value > this.dropThreshold && Rand.Chance(chanceModifier * value))
                {
                    RoofCollapserImmediate.DropRoofInCells(cell, map, null);
                }
            }
        }
    }
}
