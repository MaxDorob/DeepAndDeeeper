using RimWorld.Planet;
using RimWorld.QuestGen;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class QuestNode_Root_WandererJoin_RoofFall : QuestNode_Root_WandererJoin
    {
        public override void RunInt()
        {
            base.RunInt();
            Quest quest = QuestGen.quest;
            quest.Delay(60000, delegate
            {
                quest.End(QuestEndOutcome.Fail, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
            }, null, null, null, false, null, null, false, null, null, null, false, QuestPart.SignalListenMode.OngoingOnly, false);
        }

        public override Pawn GeneratePawn()
        {
            return this.GeneratePawn_NewTemp(null);
        }

        public override Pawn GeneratePawn_NewTemp(Map map)
        {
            Slate slate = QuestGen.slate;
            PawnGenerationRequest request;
            if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out request, false))
            {
                PawnKindDef villager = PawnKindDefOf.Villager;
                request = new PawnGenerationRequest(villager);
            }
            if (Find.Storyteller.difficulty.ChildrenAllowed)
            {
                request.AllowedDevelopmentalStages |= DevelopmentalStage.Child;
            }
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
            }
            return pawn;
        }

        public override void AddSpawnPawnQuestParts(Quest quest, Map map, Pawn pawn)
        {
            this.signalAccept = QuestGenUtility.HardcodedSignalWithQuestID("Accept");
            this.signalReject = QuestGenUtility.HardcodedSignalWithQuestID("Reject");
            CellFinder.TryFindRandomCell(map, c => !c.Fogged(map) && c.Standable(map), out var cell);
            quest.SpawnThing(map, pawn, cell: cell);
            var landslidePart = new QuestPart_EnqueueLandslide(QuestGen.slate.Get<string>("inSignal", null, false), 5.9f, 0.12f, 9, new IntRange(4, 120), cell, map);
            quest.AddPart(landslidePart);



            var damageUntilDowned = new QuestPart_DamageUntilDowned();
            damageUntilDowned.inSignal = QuestGen.slate.Get<string>("inSignal", null, false);
            damageUntilDowned.pawns.Add(pawn);
            damageUntilDowned.allowBleedingWounds = true;
            quest.AddPart(damageUntilDowned);

            if (Rand.Chance(0.33f))
            {
                HealthUtility.AdjustSeverity(pawn, HediffDefOf.Malnutrition, new FloatRange(0.2f, 0.6f).RandomInRange); // I'm too lazy to create a new one QuestPart for this
            }


            quest.Signal(this.signalAccept, delegate
            {
                quest.SetFaction(Gen.YieldSingle<Pawn>(pawn), Faction.OfPlayer, null);
                //quest.PawnsArrive(Gen.YieldSingle<Pawn>(pawn), null, map.Parent, null, false, null, null, null, null, null, false, false, true);
                quest.End(QuestEndOutcome.Success, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
            }, null, QuestPart.SignalListenMode.OngoingOnly);
            quest.Signal(this.signalReject, delegate
            {
                quest.GiveDiedOrDownedThoughts(pawn, PawnDiedOrDownedThoughtsKind.DeniedJoining, null);
                quest.End(QuestEndOutcome.Fail, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
            }, null, QuestPart.SignalListenMode.OngoingOnly);
        }

        public override void SendLetter(Quest quest, Pawn pawn)
        {
            SendLetter_NewTemp(quest, pawn, Find.AnyPlayerHomeMap);
        }
        public override void SendLetter_NewTemp(Quest quest, Pawn pawn, Map map)
        {
            TaggedString label = "LetterLabelWandererJoins".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
            TaggedString taggedString = "LetterWandererJoins".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
            QuestNode_Root_WandererJoin_RoofFall.AppendCharityInfoToLetter("JoinerCharityInfo".Translate(pawn), ref taggedString);
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref taggedString, ref label, pawn);
            QuestNode_Root_WandererJoin_RoofFall.ApplyBestSkillInfoToLetter(ref taggedString, pawn);
            ChoiceLetter_AcceptJoiner choiceLetter_AcceptJoiner = (ChoiceLetter_AcceptJoiner)LetterMaker.MakeLetter(label, taggedString, LetterDefOf.AcceptJoiner, null, null);
            choiceLetter_AcceptJoiner.signalAccept = this.signalAccept;
            choiceLetter_AcceptJoiner.signalReject = this.signalReject;
            choiceLetter_AcceptJoiner.quest = quest;
            choiceLetter_AcceptJoiner.overrideMap = map;
            choiceLetter_AcceptJoiner.StartTimeout(60000);
            Find.LetterStack.ReceiveLetter(choiceLetter_AcceptJoiner, null, 0, true);
        }

        public static void AppendCharityInfoToLetter(TaggedString charityInfo, ref TaggedString letterText)
        {
            if (ModsConfig.IdeologyActive)
            {
                IEnumerable<Pawn> source = IdeoUtility.AllColonistsWithCharityPrecept();
                if (source.Any<Pawn>())
                {
                    letterText += "\n\n" + charityInfo + "\n\n" + "PawnsHaveCharitableBeliefs".Translate() + ":";
                    foreach (IGrouping<Ideo, Pawn> grouping in from c in source
                                                               group c by c.Ideo)
                    {
                        letterText += "\n  - " + "BelieversIn".Translate(grouping.Key.name.Colorize(grouping.Key.TextColor), grouping.Select((Pawn f) => f.NameShortColored.Resolve()).ToCommaList(false, false));
                    }
                }
            }
        }

        public static void ApplyBestSkillInfoToLetter(ref TaggedString letterText, Pawn pawn)
        {
            if (pawn.skills == null || !pawn.skills.skills.Any<SkillRecord>())
            {
                return;
            }
            SkillRecord skillRecord = pawn.skills.skills.MaxBy((SkillRecord s) => s.Level);
            if (skillRecord == null)
            {
                return;
            }
            letterText += "\n\n" + "BestSkillLetterLabel".Translate(pawn.Named("PAWN")) + ": " + skillRecord.def.LabelCap + " (" + "BestSkillInfoLevel".Translate(skillRecord.Level) + ")";
        }


        private const int TimeoutTicks = 60000;

        public const float RelationWithColonistWeight = 20f;

        private string signalAccept;

        private string signalReject;
    }
}
