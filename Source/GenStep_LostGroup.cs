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
        public List<PawnKindDef> availableKindDefs;
        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            Faction faction = null;
            var pawns = new List<Pawn>();
            var count = pawnsCount.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                var pawn = PawnGenerator.GeneratePawn(availableKindDefs.RandomElement(), faction);
                pawns.Add(pawn);
                GenSpawn.Spawn(pawn, thingPos, map); 
            }
            var lord = LordMaker.MakeNewLord(faction, new LordJob_DefendPointAndAskToJoin(), map, null);
            lord.AddPawns(pawns);
            return true;
        }
    }
}
