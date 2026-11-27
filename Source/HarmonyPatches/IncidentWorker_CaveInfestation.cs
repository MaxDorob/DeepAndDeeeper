using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class IncidentWorker_CaveInfestation : IncidentWorker
    {
        // The same incident worker but ignores some default requirements
        public override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return base.CanFireNowSub(parms) && Faction.OfInsects != null && HiveUtility.TotalSpawnedHivesCount(map, false) < 30 && InfestationUtility.FindRootTunnelLoc(map, true).IsValid;
        }
        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            parms.points *= IncidentWorker_Infestation.PointsFactorCurve.Evaluate(parms.points);
            Thing t = InfestationUtility.SpawnTunnels(Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1), map, true, true, null, parms.infestationLocOverride, null);
            base.SendStandardLetter(parms, t, Array.Empty<NamedArgument>());
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}
