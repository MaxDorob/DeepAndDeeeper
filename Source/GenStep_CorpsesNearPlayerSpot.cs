using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CorpsesNearPlayerSpot : GenStep
    {
        public IntRange count = new IntRange(1, 5);
        public override int SeedPart => 910271;

        public override void Generate(Map map, GenStepParams parms)
        {
            var count = this.count.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                int age = (int)(GenDate.TicksPerHour * Rand.Range(0.5f, 2f));
                if (CellFinder.TryFindRandomCellNear(MapGenerator.PlayerStartSpot, map, 18, c => c.Standable(map) && c.GetFirstThing<Thing>(map) == null, out var loc))
                {
                    var corpse = GeneratePawnCorpse(Faction.OfPlayer, age);
                    GenSpawn.Spawn(corpse, loc, map);
                }
            }
        }
        protected Corpse GeneratePawnCorpse(Faction faction, int age)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Drifter, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false, false, false, -1, 0, false));
            pawn.Kill(null, null);
            pawn.Corpse.Age = age;
            pawn.Corpse.GetComp<CompRottable>().RotProgress += (float)pawn.Corpse.Age;
            return pawn.Corpse;
        }
    }
}
