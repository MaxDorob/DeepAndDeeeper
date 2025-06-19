using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_UndergroundTorches : GenStep
    {
        public override int SeedPart => 18268291;

        static ThingDef wallTorch;
        static Placeworker_AttachedToWall placeworker = new Placeworker_AttachedToWall();
        public override void Generate(Map map, GenStepParams parms)
        {
            wallTorch ??= DefDatabase<ThingDef>.GetNamed("TorchWallLamp");
            var count = Rand.Range(0, 6);
            List<IntVec3> cells = new List<IntVec3>();
            var onWallLimit = 160;
            for (int i = 0; i < count; i++)
            {
                if (TryFindCell(map, cells, ref onWallLimit, out var result))
                {
                    cells.Add(result);
                }
            }
            foreach (var cell in cells)
            {
                var def = ThingDefOf.TorchLamp;
                var rot = Rot4.North;
                if (TryFindRotation(wallTorch.entityDefToBuild, map, cell, out rot))
                {
                    def = wallTorch;
                }
                var torch = GenSpawn.Spawn(ThingMaker.MakeThing(def), cell, map, rot);
                if (torch.TryGetComp<CompRefuelable>(out var refuelable))
                {
                    refuelable.fuel = Rand.Range(0.03f, refuelable.Props.fuelCapacity * 0.8f);
                }
            }
        }
        private bool TryFindCell(Map map, IEnumerable<IntVec3> other, ref int onWallLimit, out IntVec3 result)
        {
            for (int i = 0; i <= 80; i++)
            {
                var potCell = CellFinderLoose.RandomCellWith(c => c.Standable(map), map, 70);
                onWallLimit--;
                if (other.Any(x => x.InHorDistOf(potCell, 8f)))
                {
                    continue;
                }
                if (onWallLimit > 0 && !TryFindRotation(wallTorch.entityDefToBuild, map, potCell, out _))
                {
                    continue;
                }
                result = potCell;
                return true;
            }
            result = IntVec3.Zero;
            return false;
        }
        private bool TryFindRotation(BuildableDef def, Map map, IntVec3 cell, out Rot4 result)
        {
            foreach (var rot in Rot4.AllRotations)
            {
                if (placeworker.AllowsPlacing(def, cell, rot, map))
                {
                    result = rot;
                    return true;
                }
            }
            result = Rot4.Invalid;
            return false;
        }
    }
}
