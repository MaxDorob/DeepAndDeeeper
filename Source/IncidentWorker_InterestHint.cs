using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class IncidentWorker_InterestHint : IncidentWorker
    {
        public override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && HiddenInterestPoint(parms.target as Map).Any();
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = parms.target as Map;
            var targets = HiddenInterestPoint(map).ToList();
            if (!targets.Any())
            {
                return false;
            }
            Pawn pawn = null;
            IntVec3 target = IntVec3.Invalid;
            target = targets.RandomElementByWeight(t => 1 / t.Position.DistanceTo(map.mapPawns.FreeColonistsAndPrisonersSpawned.MinBy(x => x.Position.DistanceTo(t.Position)).Position)).Position;
            pawn = map.mapPawns.FreeColonistsAndPrisonersSpawned.MinBy(x => x.Position.DistanceTo(target));
            foreach (var cellInLine in GenSight.PointsOnLineOfSight(target, pawn.Position))
            {
                if (!cellInLine.Fogged(map))
                {
                    target = cellInLine;
                    break;
                }
            }


            if (!target.IsValid)
            {
                return false;
            }


            var letter = Rand.Bool ? 
                LetterMaker.MakeLetter("ShashlichnikCaveHintSound".Translate(pawn.Named("PAWN")), "ShashlichnikCaveHintSoundDesc".Translate(pawn.Named("PAWN")), LetterDefOf.NeutralEvent) :
                LetterMaker.MakeLetter("ShashlichnikCaveHintMarks".Translate(pawn.Named("PAWN")), "ShashlichnikCaveHintMarksDesc".Translate(pawn.Named("PAWN")), LetterDefOf.NeutralEvent);
            letter.lookTargets = new LookTargets(target, map);
            Find.LetterStack.ReceiveLetter(letter);
            return true;
        }

        protected IEnumerable<Thing> HiddenInterestPoint(Map map)
        {
            foreach (var caveExit in map.listerThings.GetThingsOfType<CaveExit>().Where(e => e.Fogged()))
            {
                yield return caveExit;
            }
            var caskets = map.listerThings.GetThingsOfType<Building_CryptosleepCasket>().Where(c => c.Fogged());
            var groupedCaskets = caskets.OfType<Building_AncientCryptosleepCasket>().GroupBy(c => c.groupID);
            foreach (var cryptosleepPod in groupedCaskets)
            {
                yield return cryptosleepPod.RandomElement();
            }

            foreach (var cryptosleepPod in caskets.Except(groupedCaskets.SelectMany(x => x)))
            {
                yield return cryptosleepPod;
            }

            foreach (var pawn in map.mapPawns.AllPawnsSpawned.Where(p => p.Fogged()))
            {
                yield return pawn;
            }
        }
    }
}
