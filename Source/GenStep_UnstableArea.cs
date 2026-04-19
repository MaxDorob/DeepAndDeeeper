using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Shashlichnik
{
    public class GenStep_UnstableArea : GenStep
    {
        public float threshold = 0.4f;
        public override int SeedPart => 7548953;

        public override void Generate(Map map, GenStepParams parms)
        {
            var comp = map.GetComponent<CaveEntranceTracker>();
            if (comp == null)
            {
                Log.Error($"There's no CaveEntranceTracker at {map}");
                return;
            }
            var directionNoise = new Perlin(0.0249, 0.4, 1.68, 2, Rand.Int, QualityMode.Medium);
            foreach (var cell in map.AllCells)
            {
                var value = directionNoise.GetValue(cell.x, 0f, cell.z);
                if (value > threshold)
                {
                    comp.ConstantlyBlockedCells.Add(cell);
                }
            }
        }
    }
}
