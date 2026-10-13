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
            var points = HiddenInterestPoint(map).ToList();
            if (!points.Any())
            {
                return false;
            }
            Pawn pawn = null;
            IntVec3 target = IntVec3.Invalid;
            float dist = float.MaxValue;
            foreach (var t in points)
            {
                var p = map.mapPawns.FreeColonistsAndPrisonersSpawned.MinBy(x => x.Position.DistanceTo(t));
                var d = p.Position.DistanceTo(t);
                if (d < dist)
                {
                    pawn = p;
                    target = t;
                    dist = d;
                }
            }
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


            var letter = LetterMaker.MakeLetter("ShashlichnikCaveHintSound".Translate(pawn.Named("PAWN")), "ShashlichnikCaveHintSoundDesc".Translate(pawn.Named("PAWN")), LetterDefOf.NeutralEvent);
            letter.lookTargets = new LookTargets(target, map);
            Find.LetterStack.ReceiveLetter(letter);
            return true;
        }

        protected IEnumerable<IntVec3> HiddenInterestPoint(Map map)
        {

            var caveExits = map.listerThings.GetThingsOfType<CaveExit>().Where(e => e.Fogged()).ToList();
            foreach (var caveExit in caveExits)
            {
                yield return caveExit.Position;
            }
        }
    }
}
