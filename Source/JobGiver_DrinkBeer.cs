using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Analytics;
using Verse;
using Verse.AI;

namespace Shashlichnik
{
    public class JobGiver_DrinkBeer : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food != null)
            {
                if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.AlcoholHigh) == null)
                {
                    return 9.5f;
                }
            }

            return 0f;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.AlcoholHigh) != null)
            {
                return null;
            }
            Need_Food food = pawn.needs.food;
            Thing beer = pawn.inventory?.innerContainer.FirstOrDefault(x => x.def == DefsOf.Beer) ?? GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(DefsOf.Beer), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn));
            if (beer != null)
            {
                var job = JobMaker.MakeJob(JobDefOf.Ingest, beer);
                job.count = 1;
                return job;
            }
            return null;
        }
    }
}
