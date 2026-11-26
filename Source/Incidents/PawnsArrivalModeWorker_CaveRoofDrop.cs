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
    public class PawnsArrivalModeWorker_CaveRoofDrop : PawnsArrivalModeWorker_EdgeWalkIn
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            base.Arrive(pawns, parms);
            //Spawn roof drops around
            var map = parms.target as Map;
            foreach (var cell in GenRadial.RadialCellsAround(parms.spawnCenter, 6, true))
            {
                if (!cell.TryGetFirstThing<Pawn>(map, out _) && cell.Walkable(map) && Rand.Chance(0.35f))
                {
                    var caveComp = map.GetComponent<CaveMapComponent>();
                    if (caveComp != null)
                    {
                        caveComp.QueueSingleLandslide(cell, Rand.Range(5, 100));
                    }
                    else
                    {
                        RoofCollapserImmediate.DropRoofInCells(cell, map, null);
                    }
                }
            }
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            var map = (Map)parms.target;
            return CellFinder.TryFindRandomCell(map, (c) => !c.Fogged(map) && c.Standable(map), out parms.spawnCenter);
        }
    }
}
