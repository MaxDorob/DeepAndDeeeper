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
    public class GenStep_LostGroup : GenStep_CaveInterest_LostPawn
    {
        public override int SeedPart => 828100;

        public GenStep_LostGroup()
        {
            this.countChances = [
                new CountChance(){
                    count = 0,
                    chance = 0.05f
                },
                new CountChance(){
                    count= 1,
                    chance = 0.95f
                }
                ];
            this.availableKindDefs = [PawnKindDefOf.Colonist, DefsOf.ShashlichnikDeepDiver, PawnKindDefOf.Slave];
        }

        public IntRange pawnsCount = new IntRange(1, 3);
        public IntRange mealCount = new IntRange(3, 9);
        public List<PawnKindDef> availableKindDefs;
        protected virtual LordJob CreateLordJob() => new LordJob_DefendPointAndAskToJoin();
        protected virtual Faction GetRandomGroupFaction() => null;
        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            Faction faction = GetRandomGroupFaction();
            var pawns = new List<Pawn>();
            var count = pawnsCount.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                var pawn = PawnGenerator.GeneratePawn(faction?.RandomPawnKind() ?? availableKindDefs.RandomElement(), faction);
                pawns.Add(pawn);
                GenSpawn.Spawn(pawn, thingPos, map);
                PostProcessPawn(pawn);
            }
            var lord = LordMaker.MakeNewLord(faction, CreateLordJob(), map, null);
            lord.AddPawns(pawns);
            return true;
        }
        protected virtual void PostProcessPawn(Pawn pawn)
        {
            var count = mealCount.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                PawnInventoryGenerator.GiveRandomFood(pawn);
            }
            PawnInventoryGenerator.GiveDrugsIfAddicted(pawn);
        }
    }
}
