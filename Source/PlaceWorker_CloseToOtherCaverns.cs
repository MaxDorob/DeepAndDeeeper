using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class PlaceWorker_CloseToOtherCaverns : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (map.listerThings.ThingsInGroup(ThingRequestGroup.MapPortal).OfType<CaveEntrance>().Any(c => c.Position.InHorDistOf(loc, CaveEntranceTracker.blockRadius)) ||
                map.listerThings.ThingsInGroup(ThingRequestGroup.Blueprint).OfType<Blueprint_Build>().Any(x => x.BuildDef.thingClass == typeof(CaveEntrance) && x.Position.InHorDistOf(loc, CaveEntranceTracker.blockRadius)) ||
                map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame).OfType<Frame>().Any(x=>x.BuildDef.thingClass == typeof(CaveEntrance) && x.Position.InHorDistOf(loc, CaveEntranceTracker.blockRadius)))
                return new AcceptanceReport("ShashlichnikCloseToOtherCavern".Translate());
            return true;
        }
    }
}
