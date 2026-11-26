using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace Shashlichnik
{
    public class LordJob_DefendPointAndAskToJoin : LordJob_DefendPoint, ISignalReceiver
    {
        public bool sentLetter = false;
        public override void PostCleanup()
        {
            base.PostCleanup();
            Find.SignalManager.DeregisterReceiver(this);
        }
        public override void Notify_AddedToLord()
        {
            base.Notify_AddedToLord();
            Find.SignalManager.RegisterReceiver(this);
        }
        public override void LordJobTick()
        {
            base.LordJobTick();
            if (!sentLetter)
            {
                if (lord.ownedPawns.Any(p => !p.PositionHeld.Fogged(Map)))
                {
                    var pawn = lord.ownedPawns.First(p => !p.PositionHeld.Fogged(Map));
                    TaggedString label = "ShashlichnikLetterLabelLostGroupJoins".Translate(pawn);
                    TaggedString text = "ShashlichnikTextLabelLostGroupJoins".Translate(pawn);
                    Log.Warning("Pawns wants to join");
                    var letter = (ChoiceLetter_AcceptLostGroup)LetterMaker.MakeLetter(label, text, DefsOf.ShashlichnikLostGroupWantsToJoin, pawn);
                    letter.signalAccept = this.AcceptSignal;
                    letter.signalReject = this.RejectSignal;
                    Find.LetterStack.ReceiveLetter(letter);
                    sentLetter = true;
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref sentLetter, nameof(sentLetter));
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Find.SignalManager.RegisterReceiver(this);
            }
        }
        private string AcceptSignal => $"{lord.GetUniqueLoadID()}.accept";
        private string RejectSignal => $"{lord.GetUniqueLoadID()}.reject";
        public void Notify_SignalReceived(Signal signal)
        {
            var notHostile = lord.ownedPawns.Where(p => !p.HostileTo(Faction.OfPlayer)).ToList();
            if (!notHostile.Any())
            {
                return;
            }
            if (signal.tag == AcceptSignal)
            {
                foreach (var pawn in notHostile)
                {
                    RecruitUtility.Recruit(pawn, Faction.OfPlayer);
                    lord.RemovePawn(pawn);
                }
            }
            else if (signal.tag == RejectSignal)
            {

            }
        }
    }
}
