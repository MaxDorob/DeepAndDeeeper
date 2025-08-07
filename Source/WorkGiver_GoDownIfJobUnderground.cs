using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace Shashlichnik
{
    public abstract class WorkGiver_GoDownIfJobUnderground : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(DefsOf.ShashlichnikCaveEntrance);
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is CaveEntrance entrance && entrance.autoEnter && entrance.caveExit != null && (!entrance.caveExit.exitIfNoJob || pawn.timetable?.CurrentAssignment != TimeAssignmentDefOf.Sleep) && TryFindFirstAvailableJobTargetAt(entrance.caveExit, pawn, out var targetThing))
            {
                return JobMaker.MakeJob(DefsOf.ShashlichnikEnterPortalForJob, t, targetThing);
            }
            return null;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !pawn.Map.listerBuildings.AllColonistBuildingsOfType<CaveEntrance>().Any(x => x.autoEnter);
        }
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is CaveEntrance entrance && entrance.autoEnter && entrance.caveExit != null && (!entrance.caveExit.exitIfNoJob || pawn.timetable?.CurrentAssignment != TimeAssignmentDefOf.Sleep) && TryFindFirstAvailableJobTargetAt(entrance.caveExit, pawn, out var targetThing))
            {
                if (TryFindFirstAvailableJobTargetAt(entrance.caveExit, pawn, out _))
                {
                    return true;
                }
            }
            return false;
        }
        protected bool TryFindFirstAvailableJobTargetAt(CaveExit caveExit, Pawn forPawn, out Thing result)
        {
            Map map = caveExit.Map;
            IntVec3 startPos = caveExit.caveEntrance?.GetDestinationLocation() ?? caveExit.Position;
            foreach (var target in PotentialTargetsUnderground(caveExit))
            {
                if (IsTargetAvailable(target, map, startPos, forPawn))
                {
                    result = target;
                    return true;
                }
            }

            result = null;
            return false;
        }
        protected virtual bool IsTargetAvailable(Thing target, Map map, IntVec3 startPos, Pawn forPawn)
        {
            return !target.IsForbidden(forPawn) &&
                        !IsAlreadyReserved(target) &&
                        (!forPawn.playerSettings.allowedAreas.TryGetValue(map, out var caveArea) || caveArea[target.Position]) &&
                        map.reachability.CanReach(startPos, target.Position, PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.ByPawn));
        }
        protected bool IsAlreadyReserved(Thing target)
        {
            if (target?.Map?.reservationManager?.TryGetReserver(target, Faction.OfPlayer, out var reserver) ?? false)
            {
                return true;
            }
            return false;
        }
        protected abstract IEnumerable<Thing> PotentialTargetsUnderground(CaveExit caveExit);
    }
}
