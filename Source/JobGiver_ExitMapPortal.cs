using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace Shashlichnik
{
    public class JobGiver_ExitMapPortal : JobGiver_ExitMapBest
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var job = base.TryGiveJob(pawn);
            if (job != null)
            {
                return job;
            }
            else
            {
                if (!this.TryFindGoodExitDest(pawn, true, this.canBash, out var c))
                {
                    return null;
                }

                using (PawnPath pawnPath = pawn.Map.pathFinder.FindPathNow(pawn.Position, c, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassAllDestroyableThings, false, false, false, true), null, PathEndMode.OnCell, null))
                {
                    IntVec3 cellBeforeBlocker;
                    Thing thing = pawnPath.FirstBlockingBuilding(out cellBeforeBlocker, pawn);
                    if (thing != null)
                    {
                        job = DigUtility.PassBlockerJob(pawn, thing, cellBeforeBlocker, true, true);
                        if (job != null)
                        {
                            return job;
                        }
                    }
                }
                return null;
            }
        }
        public override bool TryFindGoodExitDest(Pawn pawn, bool canDig, bool canBash, out IntVec3 dest)
        {
            var exits = pawn.Map.listerThings.GetThingsOfType<CaveExit>().Where(x => x.caveEntrance != null);
            Log.Message($"Trying to find exit. Found any {exits.Any()} ({exits.FirstOrDefault()?.Position})");
            if (exits.Any())
            {
                dest = exits.First().Position;
                return true;
            }
            return base.TryFindGoodExitDest(pawn, canDig, canBash, out dest);
        }
    }
}
