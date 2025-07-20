using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class WorkGiver_GoDownIfJobUnderground_Haul : WorkGiver_GoDownIfJobUnderground
    {
        protected override IEnumerable<Thing> PotentialTargetsUnderground(CaveExit caveExit)
        {
            return caveExit.Map.listerHaulables.ThingsPotentiallyNeedingHauling();
        }

        protected override bool IsTargetAvailable(Thing target, Map map, IntVec3 startPos, Pawn forPawn)
        {
            return base.IsTargetAvailable(target, map, startPos, forPawn) && StoreUtility.TryFindBestBetterStorageFor(target, null, map, StoreUtility.CurrentStoragePriorityOf(target), forPawn.Faction, out _, out _, false);
        }
    }
}
