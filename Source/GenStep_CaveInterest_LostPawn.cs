using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_LostPawn : GenStep_CaveInterest
    {
        public GenStep_CaveInterest_LostPawn()
        {
            this.countChances = [
                new CountChance(){
                    count = 0,
                    chance = 0.95f
                },
                new CountChance(){
                    count= 1,
                    chance = 0.05f
                }
                ];
            this.subCountChances = [new CountChance() { count = 1, chance = 1f }];
        }

        public override int SeedPart => 2901072;

        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            var faction = Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            var pawn = PawnGenerator.GeneratePawn(faction.RandomPawnKind(), faction);
            pawn.needs.TryGetNeed<Need_Food>().CurLevel *= Rand.Range(0.1f, 0.6f);
            GenSpawn.Spawn(pawn, thingPos, map);
            return true;
        }
    }
}
