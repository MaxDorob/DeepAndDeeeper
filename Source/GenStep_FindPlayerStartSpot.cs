using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_FindPlayerStartSpot : RimWorld.GenStep_FindPlayerStartSpot
    {
        public override int SeedPart => 9276172;

        public override void Generate(Map map, GenStepParams parms)
        {
            var exit = map.listerThings.ThingsOfDef(DefsOf.ShashlichnikCaveExit).FirstOrDefault();
            if (exit == null)
            {
                Log.Error($"Cannot find an exit");
                MapGenerator.PlayerStartSpot = IntVec3.Invalid;
                base.Generate(map, parms);
                return;
            }
            var usedRects = MapGenerator.GetOrGenerateVar<List<CellRect>>("UsedRects");
            IntVec3 spot;
            if (!CellFinderLoose.TryGetRandomCellWith(c => !IsInsideAnyCellRect(usedRects, c) && !map.reachability.CanReach(c, exit.InteractionCell, Verse.AI.PathEndMode.ClosestTouch, TraverseMode.ByPawn | TraverseMode.PassDoors, Danger.Deadly) && c.Standable(map),
                map,
                50,
                out spot))
            {
                Log.Warning("Cannot find standable position");
                if (!CellFinderLoose.TryGetRandomCellWith(c => !IsInsideAnyCellRect(usedRects, c) && !map.reachability.CanReach(c, exit.InteractionCell, Verse.AI.PathEndMode.ClosestTouch, TraverseMode.ByPawn | TraverseMode.PassDoors, Danger.Deadly),
                    map,
                    50,
                    out spot))
                {
                    Log.Warning($"Failed to find any position without access to the exit");
                    spot = CellFinderLoose.RandomCellWith(c => !IsInsideAnyCellRect(usedRects, c) && exit.Position.DistanceTo(c) > 30f, map);
                }
            }

            MapGenerator.PlayerStartSpot = spot;

            foreach (var cell in GenRadial.RadialCellsAround(spot, 5.5f, true))
            {
                if (cell.DistanceTo(spot) < 3.4f)
                {
                    foreach(var thing in map.thingGrid.ThingsListAt(cell).Where(t => t.def.passability != Traversability.Standable).ToList())
                    {
                        thing.Destroy();
                    }
                }
                else
                {
                    var chance = 0.6f;
                    if (cell.GetEdifice(map) != null)
                    {
                        chance = 0.06f;
                    }
                    if (Rand.Chance(chance))
                    {
                        RoofCollapserImmediate.DropRoofInCells(cell, map, null);
                    }
                }
            }
        }

        private bool IsInsideAnyCellRect(IEnumerable<CellRect> cellRects, IntVec3 cell)
        {
            return cellRects.Any(r => r.Contains(cell));
        }

    }
}
