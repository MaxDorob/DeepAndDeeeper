using RimWorld;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Shashlichnik
{
    public class GenStep_Caves // Kinda backward compatibility class (GenStep_Caves was removed in 1.6)
#if !v16
        : RimWorld.GenStep_Caves{}
#else
        : GenStep
    {
        public float branchChance;
        public float minTunnelWidth;
        public float widthOffsetPerCell;

        public override int SeedPart => 646814558;

        private Perlin directionNoise;
        public override void Generate(Map map, GenStepParams parms)
        {
            this.directionNoise = new Perlin(0.002050000010058284, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            BoolGrid visited = new BoolGrid(map);
            List<IntVec3> group = new List<IntVec3>();
            MapGenCavesUtility.CaveGenParms caveParams = MapGenCavesUtility.CaveGenParms.Default;
            caveParams.widthOffsetPerCell = widthOffsetPerCell;
            caveParams.minTunnelWidth = minTunnelWidth;
            caveParams.branchChance = branchChance;
            caveParams.allowBranchingAfterThisManyCells = 10;
            MapGenCavesUtility.GenerateCaves(map, visited, group, this.directionNoise, caveParams, c => IsRock(c, elevation, map));
        }
        public static bool IsRock(IntVec3 c, MapGenFloatGrid elevation, Map map)
        {
            return c.InBounds(map) && elevation[c] > 0.7f;
        }
    }
#endif
}
