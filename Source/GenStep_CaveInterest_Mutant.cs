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
    public class GenStep_CaveInterest_Mutant : GenStep_CaveInterest_Anomaly
    {

        public PawnKindDef mutant;

        public GenStep_CaveInterest_Mutant() : base()
        {
            countChances = [
                new CountChance() {count = 0, chance = 0.7f},
                new CountChance() {count = 1, chance = 0.3f}
                ];
            subCountChances = [
                new CountChance() {count = 1, chance = 1f}
                ];
        }

        public override int SeedPart => 927192;
        public override void Generate(Map map, GenStepParams parms)
        {
            base.Generate(map, parms);
        }
        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            var pawn = PawnGenerator.GeneratePawn(mutant, FactionUtility.DefaultFactionFrom(mutant.defaultFactionDef));
            GenSpawn.Spawn(pawn, thingPos, map);
            if (pawn.Spawned && pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
            {
                Lord lord = null;
                if (pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Any((Pawn p) => p != pawn))
                {
                    Pawn otherPawn = (Pawn)GenClosest.ClosestThing_Global(pawn.Position, pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction), 99999f, (Thing p) => p != pawn && ((Pawn)p).GetLord() != null, null, false);
                    lord = otherPawn?.GetLord();
                }
                if (lord == null || !lord.CanAddPawn(pawn))
                {
                    lord = LordMaker.MakeNewLord(pawn.Faction, new LordJob_DefendPoint(pawn.Position, null, null, false, true), map);
                }
                if (lord != null && lord.LordJob.CanAutoAddPawns)
                {
                    lord.AddPawn(pawn);
                }
            }
            return true;
        }
    }
}
