using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlichnik
{
    public class IncidentWorker_CaveRaid : IncidentWorker_RaidEnemy
    {
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            base.ResolveRaidStrategy(parms, groupKind);
            parms.raidStrategy = DefsOf.ShashlichnikCaveRaid;
        }
    }
}
