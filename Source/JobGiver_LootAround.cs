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
    public class JobGiver_LootAround : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            return 8.6f;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (TryFindBestItemToSteal(pawn.Position, pawn.Map, 9.9f, out var item, pawn))
            {
                var job = JobMaker.MakeJob(JobDefOf.TakeCountToInventory, item);
                job.count = Mathf.Min(item.stackCount, (int)(pawn.GetStatValue(StatDefOf.CarryingCapacity, true, -1) / item.def.VolumePerUnit));
                return job;
            }
            return null;
        }
        public static bool TryFindBestItemToSteal(IntVec3 root, Map map, float maxDist, out Thing item, Pawn thief)
        {
            if (!thief.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                item = null;
                return false;
            }
            Predicate<Thing> validator = (Thing t) => (thief == null || thief.CanReserve(t, 1, -1, null, false)) && t.def.stealable && !t.IsBurning();
            item = GenClosest.ClosestThing_Regionwise_ReachablePrioritized(root, map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEverOrMinifiable), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), maxDist, validator, StealAIUtility.GetValue, 15, 15);
            if (item != null && (StealAIUtility.GetValue(item) < 10f))
            {
                item = null;
            }
            return item != null;
        }
    }
}
