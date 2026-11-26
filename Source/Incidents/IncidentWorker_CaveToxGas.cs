using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Shashlichnik
{
    public class IncidentWorker_CaveToxGas : IncidentWorker
    {
        public override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = parms.target as Map;
            var gasGrid = map.gasGrid;
            if (CellFinder.TryFindRandomCell(map, c => !c.Fogged(map) && c.Walkable(map), out var cell))
            {
                gasGrid.AddGas(cell, GasType.ToxGas, Rand.Range(12000, 16000));
                base.SendStandardLetter(parms, new LookTargets(cell, map), Array.Empty<NamedArgument>());
                Find.TickManager.slower.SignalForceNormalSpeedShort();
                return true;
            }
            return false;
        }
    }
}
