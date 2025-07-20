using LudeonTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    internal static class DebugActions
    {
        [DebugAction(category = "Deep And Deeper", name = "Move all pawns")]
        public static void MoveAllPawns()
        {
            var currentMap = Find.CurrentMap;
            var mapComp = currentMap.GetComponent<CaveMapComponent>();
            if (mapComp != null)
            {
                var entrance = mapComp.caveExit?.caveEntrance;
                var moveToMap = entrance?.Map;
                if (moveToMap == null)
                {
                    foreach (var map in Find.Maps)
                    {
                        if (map.listerThings.GetThingsOfType<CaveEntrance>().Any(e => e.cave == currentMap))
                        {
                            moveToMap = map;
                            break;
                        }
                    }
                    if (moveToMap == null)
                    {
                        moveToMap = Find.AnyPlayerHomeMap;
                    }
                }
                if (moveToMap == null)
                {
                    Log.Error("Map is null");
                    return;
                }
                foreach (var pawn in currentMap.mapPawns.FreeColonistsSpawned)
                {
                    IntVec3 cell;
                    if (entrance != null)
                    {
                        cell = CellFinder.StandableCellNear(entrance.Position, moveToMap, 12);
                    }
                    else if (!CellFinderLoose.TryGetRandomCellWith(c => c.Standable(moveToMap), moveToMap, 100, out cell))
                    {
                        cell = moveToMap.Center;
                        Log.Warning($"Can't find a cell for {pawn}, using center");

                    }
                    if (!cell.Standable(moveToMap))
                    {
                        Log.Error("The cell is not standable. Skip.");
                        continue;
                    }
                    pawn.DeSpawnOrDeselect();
                    GenSpawn.Spawn(pawn, cell, moveToMap);
                }
            }
            else
            {
                Log.Warning("Current map is not cave");
            }
        }
    }
}
