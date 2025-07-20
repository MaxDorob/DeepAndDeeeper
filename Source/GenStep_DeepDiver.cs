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
    public class GenStep_DeepDiver : GenStep_CaveInterest_LostPawn
    {
        public override int SeedPart => 828100;

        public GenStep_DeepDiver()
        {
            this.countChances = [
                new CountChance(){
                    count = 0,
                    chance = 0.85f
                },
                new CountChance(){
                    count= 1,
                    chance = 0.15f
                }
                ];
            //this.subCountChances = [new CountChance() { count = 1, chance = 1f }];
        }


        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            Faction faction = null;
            if (DefsOf.OutlanderRoughStoneborn != null)
            {
                faction = Find.FactionManager.FirstFactionOfDef(DefsOf.OutlanderRoughStoneborn);
            }

            var pawn = PawnGenerator.GeneratePawn(DefsOf.ShashlichnikDeepDiver, faction);
            GenSpawn.Spawn(pawn, thingPos, map);
            var lord = LordMaker.MakeNewLord(pawn.Faction, new LordJob_MineClusters(), Find.CurrentMap, null);
            lord.AddPawn(pawn);
            return true;
        }
    }
}
