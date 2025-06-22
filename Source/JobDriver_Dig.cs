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
    public class JobDriver_Dig : JobDriver
    {
        private Effecter graveDigEffect;

        public CaveEntrance CaveEntrance => TargetA.Thing as CaveEntrance;
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOn(() => CaveEntrance.TicksToOpen <= 0 || CaveEntrance.IsCollapsing);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            Toil digToil = ToilMaker.MakeToil("MakeNewToils");
            digToil.defaultCompleteMode = ToilCompleteMode.Never;
            digToil.tickAction = DigCaveEntrance;
            yield return digToil.WithProgressBar(TargetIndex.A, () => 1f - ((float)CaveEntrance.TicksToOpen - 1) / CaveEntrance.tickToOpenConst);
        }
        private void DigCaveEntrance()
        {
            CaveEntrance.TicksToOpen--;
            if (pawn.IsHashIntervalTick(80))
            {
                graveDigEffect = Rand.Bool ? EffecterDefOf.BuryPawn.Spawn() : EffecterDefOf.Mine.Spawn();
                graveDigEffect.Trigger(CaveEntrance, CaveEntrance, -1);
            }
            if (CaveEntrance.TicksToOpen % 900 == 0)
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.Mined, pawn.Named(HistoryEventArgsNames.Doer)), true);
            }
            graveDigEffect?.EffectTick(CaveEntrance, CaveEntrance);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
        }
    }
}
