using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_CorpsePile : GenStep_CaveInterest
    {
        protected virtual Corpse GeneratePawnCorpse(Faction faction, int age)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Drifter, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false, false, false, -1, 0, false));
            pawn.Kill(null, null);
            pawn.Corpse.Age = age;
            pawn.Corpse.GetComp<CompRottable>().RotProgress += (float)pawn.Corpse.Age;
            return pawn.Corpse;
        }
        protected override bool TrySpawnInterestAt(Map map, IntVec3 cell)
        {
            int randomInRange = CorpseCountRange.RandomInRange;
            Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out var faction, true, false, TechLevel.Undefined, false);
            int age = Mathf.RoundToInt((float)(CorpseAgeRangeDays.RandomInRange * 60000));
            for (int i = 0; i < randomInRange; i++)
            {                
                if (CellFinder.TryFindRandomCellNear(cell, map, CorpseSpawnRadius, c => c.Standable(map) && c.GetFirstThing<Thing>(map) == null, out var loc))
                {
                    Spawn(GeneratePawnCorpse(faction, age), map, loc);
                }
            }
            return true;
        }
        protected virtual void Spawn(Corpse corpse, Map map, IntVec3 loc)
        {
            GenSpawn.Spawn(corpse, loc, map, WipeMode.Vanish);
        }
        public int CorpseSpawnRadius = 4;
        public IntRange CorpseCountRange = new IntRange(3, 6);
        public IntRange CorpseAgeRangeDays = new IntRange(15, 120);

        public override int SeedPart => 8727653;
    }
}
