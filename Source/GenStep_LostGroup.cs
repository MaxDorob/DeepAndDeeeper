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
            //this.subCountChances = [new CountChance() { count = 1, chance = 1f }];
        }

        
        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            Faction faction = null;
            var pawns = new List<Pawn>();
            for (int i = 0; i < 3; i++)
            {
                var pawn = PawnGenerator.GeneratePawn(DefsOf.ShashlichnikDeepDiver, faction);
                pawns.Add(pawn);
                GenSpawn.Spawn(pawn, thingPos, map); 
            }
            var lord = LordMaker.MakeNewLord(faction, new LordJob_DefendPointAndAskToJoin(), map, null);
            lord.AddPawns(pawns);
            return true;
        }
    }
}
