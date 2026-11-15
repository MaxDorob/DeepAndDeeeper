using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class PawnsArrivalModeWorker_CaveEdgeWalkIn : PawnsArrivalModeWorker_EdgeWalkIn
    {
        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            var map = parms.target as Map;
            if (CellFinder.TryFindRandomEdgeCellWith(c => !c.Fogged(map) && c.Standable(map), map, CellFinder.EdgeRoadChance_Hostile, out parms.spawnCenter))
            {
                parms.spawnRotation = Rot4.FromAngleFlat((map.Center - parms.spawnCenter).AngleFlat);
                return true;
            }
            return false;
        }
    }
}
