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
    [HarmonyLib.HarmonyPatch(typeof(JobGiver_ExitMap), nameof(JobGiver_ExitMap.TryGiveJob))]
    internal static class JobGiver_ExitMap_Patch //Similar functionally was added at 1.6 tho
    {
        public static bool Prefix(Pawn pawn, ref Job __result)
        {
            var caveComp = pawn?.MapHeld?.GetComponent<CaveMapComponent>();
            if (caveComp != null)
            {
                if (pawn.Downed && !pawn.Crawling)
                {
                    return false;
                }
                foreach (Thing portal in pawn.MapHeld.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.MapPortal)))
                {
                    if (pawn.CanReach(portal, PathEndMode.Touch, Danger.Deadly, false, false, TraverseMode.ByPawn))
                    {
                        __result = JobMaker.MakeJob(JobDefOf.EnterPortal, portal);
                        return false;
                    }
                }
                return false;
            }
            return true;
        }

    }
}
