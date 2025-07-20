using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class SpammerComp : ThingComp
    {
        private int nextMessageTick = -9999999;
        public int messagesLeft = -1;

        Spammer_CompProperties Props => (Spammer_CompProperties)props;

        public override void CompTick()
        {
            base.CompTick();
            if (messagesLeft != 0 && !parent.Position.Fogged(parent.Map) && nextMessageTick < Find.TickManager.TicksGame)
            {
                if (messagesLeft < 0)
                {
                    messagesLeft = Props.totalMessagesCount;
                }
                var spammers = parent.Map.mapPawns.FreeColonists.Where(p => p.Position.InHorDistOf(parent.Position, Props.distance)).ToList();
                if (spammers.Count > 0)
                {
                    var letter = LetterMaker.MakeLetter("WeAreRich".Translate(), "WeAreRich".Translate(), Props.totalMessagesCount - messagesLeft > Props.messagesBeforeShutUp ? DefsOf.ShashlichnikPositiveEventShutUp : LetterDefOf.PositiveEvent, new LookTargets((new Thing[] { parent }).Union(spammers)));
                    Find.LetterStack.ReceiveLetter(letter);
                    messagesLeft--;
                    nextMessageTick = Find.TickManager.TicksGame + Props.tickBeforeNextMessage.RandomInRange;
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextMessageTick, nameof(nextMessageTick));
            Scribe_Values.Look(ref messagesLeft, nameof(messagesLeft));
        }
    }
    public class Spammer_CompProperties : CompProperties
    {
        public Spammer_CompProperties() : base()
        {
            compClass = typeof(SpammerComp);
        }
        public int totalMessagesCount = 12;
        public int messagesBeforeShutUp = 5;
        public IntRange tickBeforeNextMessage = new IntRange(200, 450);
        public float distance = 7.9f;
    }
}
