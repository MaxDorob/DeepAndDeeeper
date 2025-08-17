using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Shashlichnik
{
    public class JobGiver_DigClosestCluster : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            return 8.5f;
        }
        protected bool TryFindClosestOre(Pawn pawn, out Thing ore)
        {
            ore = pawn.Map.listerThings.GetThingsOfType<Mineable>().Where(x => x.def != ThingDefOf.MineableSteel && x.def.building.isResourceRock && pawn.CanReserve(x)).OrderBy(m => (m.Position - pawn.Position).LengthHorizontalSquared).FirstOrDefault();
            return ore != null;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (TryFindClosestOre(pawn, out Thing ore))
            {
                if (pawn.CanReach(ore, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    return JobMaker.MakeJob(JobDefOf.Mine, ore, 20000, true);
                }
                else
                {
                    using (PawnPath pawnPath = pawn.Map.pathFinder.
#if v16
                        FindPathNow
#else
                        FindPath
#endif
                        (pawn.Position, ore.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassAllDestroyableThings)))
                    {
                        IntVec3 cellBeforeBlocker;
                        Thing thing = pawnPath.FirstBlockingBuilding(out cellBeforeBlocker, pawn);
                        if (thing != null)
                        {
                            return DigUtility.PassBlockerJob(pawn, thing, cellBeforeBlocker, true, true);
                        }
                    }
                }
            }
            return null;
        }
    }
}
