using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Shashlichnik
{
    public static class JobGiver_Patch
    {
        public static string[] overridingJobDefNames = new string[] { "LayDown" };

        private static void TryOverrideJob(Pawn pawn, ref Job job)
        {
            // Log.Message($"job is [{job?.def?.defName}]");
            if (job?.def?.defName is { } val
                && overridingJobDefNames.Contains(val))
            {
                var caveComp = pawn.Map.GetComponent<CaveMapComponent>();
                if (caveComp != null && caveComp.caveExit != null && caveComp.caveExit.Spawned &&
                    caveComp.caveExit.exitIfNoJob)
                {
                    Log.Message($"Detected overridable job [{val}]. Redirecting the pawn to exit cave instead.");
                    var oldjob = job;
                    JobMaker.ReturnToPool(oldjob); // Return the old job to the pool
                    // make a new one
                    job = JobMaker.MakeJob(JobDefOf.EnterPortal, caveComp.caveExit);
                }
            }
        }

        // patch return result
        [HarmonyLib.HarmonyPatch(typeof(JobGiver_GetRest), nameof(JobGiver_GetRest.TryGiveJob))]
        internal static class GetRest_TryGiveJob_Patch
        {
            public static void Postfix(Pawn pawn, ref Job __result)
            {
                TryOverrideJob(pawn, ref __result);
            }
        }
    }
}