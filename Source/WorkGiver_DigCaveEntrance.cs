using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Shashlichnik
{
    public class WorkGiver_DigCaveEntrance : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(DefsOf.CaveEntrance);
            }
        }
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            var entries = pawn.Map.listerBuildings.AllBuildingsColonistOfDef(DefsOf.CaveEntrance).OfType<CaveEntrance>();
            return !entries.Any(b => (b as CaveEntrance).TicksToOpen > 0 && !b.IsCollapsing);
        }
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction)
            {
                return false;
            }
            return t is CaveEntrance caveEntrance && pawn.CanReserve(caveEntrance) && caveEntrance.TicksToOpen > 0 && !caveEntrance.IsCollapsing;
        }
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefsOf.ShashlichnikDigCaveEntrance, t, expiryInterval: 1500);
        }
    }
}
