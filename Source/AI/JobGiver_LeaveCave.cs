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
    public class JobGiver_LeaveCave : ThinkNode
    {
        public override float GetPriority(Pawn pawn)
        {
            if (pawn.workSettings == null || !pawn.workSettings.EverWork)
            {
                return 10f;
            }
            TimeAssignmentDef timeAssignmentDef = pawn.timetable?.CurrentAssignment ?? TimeAssignmentDefOf.Anything;
            if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
            {
                return 5.4f;
            }
            if (timeAssignmentDef == TimeAssignmentDefOf.Work)
            {
                return 8.9f;
            }
            if (timeAssignmentDef == TimeAssignmentDefOf.Sleep)
            {
                return 10f;
            }
            if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
            {
                return 10f;
            }
            if (timeAssignmentDef == TimeAssignmentDefOf.Meditate)
            {
                return 10f;
            }
            Log.WarningOnce("There are unknown TimeAssignmentDef: " + timeAssignmentDef.defName, 8281037);
            return 0f;
        }
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            var caveComp = pawn.Map.GetComponent<CaveMapComponent>();
            if (caveComp != null)
            {
                if (caveComp.caveExit != null && caveComp.caveExit.Spawned && caveComp.caveExit.exitIfNoJob)
                {
                    var job = JobMaker.MakeJob(JobDefOf.EnterPortal, caveComp.caveExit);
                    return new ThinkResult(job, this);
                }
            }
            return ThinkResult.NoJob;
        }
    }
}
