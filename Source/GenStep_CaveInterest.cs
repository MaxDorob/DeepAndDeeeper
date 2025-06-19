using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 232323257;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            int randomInRange = InterestPointCountRange.RandomInRange;
            List<IntVec3> interestPoints = new List<IntVec3>();
            for (int i = 0; i < randomInRange; i++)
            {
                if (CellFinder.TryFindRandomCell(map, c => c.Standable(map) && c.DistanceToEdge(map) > 5 && !interestPoints.Any((IntVec3 p) => c.InHorDistOf(p, MinDistApart)), out var item))
                {
                    interestPoints.Add(item);
                }
            }
            foreach (IntVec3 intVec in interestPoints)
            {
                GenStep_CaveInterest.CaveInterestKind underCaveInterestKind = Gen.RandomEnumValue<GenStep_CaveInterest.CaveInterestKind>(false);
                foreach (IntVec3 c2 in GridShapeMaker.IrregularLump(intVec, map, InterestPointSize, null))
                {
                    foreach (Thing thing in c2.GetThingList(map).ToList<Thing>())
                    {
                        if (thing.def.destroyable)
                        {
                            Building edifice = c2.GetEdifice(map);
                            bool? flag;
                            if (edifice == null)
                            {
                                flag = null;
                            }
                            else
                            {
                                ThingDef def = edifice.def;
                                if (def == null)
                                {
                                    flag = null;
                                }
                                else
                                {
                                    BuildingProperties building = def.building;
                                    flag = ((building != null) ? new bool?(building.isNaturalRock) : null);
                                }
                            }
                            if (!(flag ?? false))
                            {
                                Building edifice2 = c2.GetEdifice(map);
                                if (((edifice2 != null) ? edifice2.def : null) != ThingDefOf.Fleshmass)
                                {
                                    continue;
                                }
                            }
                            thing.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
                switch (underCaveInterestKind)
                {
                    case CaveInterestKind.MushroomPatch:
                        GenerateMushroomPatch(map, intVec);
                        break;
                    case CaveInterestKind.ChemfuelGenerator:
                        GenerateChemfuel(map, intVec);
                        break;
                    case CaveInterestKind.InsectHive:
                        GenerateHive(map, intVec);
                        break;
                    case CaveInterestKind.CorpseGear:
                        GenerateCorpseGear(map, intVec);
                        break;
                    case CaveInterestKind.CorpsePile:
                        GenerateCorpsePile(map, intVec);
                        break;
                    case CaveInterestKind.SleepingFleshbeasts:
                        if (!ModsConfig.AnomalyActive)
                        {
                            goto case CaveInterestKind.CorpseGear;
                        }
                        GenerateSleepingFleshbeasts(map, intVec);
                        break;
                }
            }
        }

        private void GenerateMushroomPatch(Map map, IntVec3 cell)
        {
            List<ThingDef> source = new List<ThingDef>
            {
                ThingDefOf.Plant_HealrootWild,
                ThingDefOf.Glowstool,
                ThingDefOf.Bryolux,
                ThingDefOf.Agarilux
            };
            foreach (IntVec3 intVec in GridShapeMaker.IrregularLump(cell, map, PatchSizeRange.RandomInRange, null))
            {
                map.terrainGrid.SetTerrain(intVec, TerrainDefOf.SoilRich);
                if (intVec.GetPlant(map) == null && intVec.GetCover(map) == null && intVec.GetEdifice(map) == null && Rand.Chance(PatchDensity))
                {
                    ((Plant)GenSpawn.Spawn(ThingMaker.MakeThing(source.RandomElement(), null), intVec, map, WipeMode.Vanish)).Growth = Mathf.Clamp01(WildPlantSpawner.InitialGrowthRandomRange.RandomInRange);
                }
            }
        }

        private void GenerateChemfuel(Map map, IntVec3 cell)
        {
            GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.AncientGenerator ?? ThingDefOf.ChemfuelPoweredGenerator, null), cell, map, WipeMode.Vanish);
            int randomInRange = ChemfuelCountRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                if (CellFinder.TryFindRandomCellNear(cell, map, ChemfuelStackMaxDist, c => c.Standable(map), out var loc))
                {
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Chemfuel, null);
                    thing.stackCount = ChemfuelStackCountRange.RandomInRange;
                    GenSpawn.Spawn(thing, loc, map, WipeMode.Vanish);
                }
            }
            foreach (IntVec3 c2 in GridShapeMaker.IrregularLump(cell, map, ChemfuelPuddleSize, null))
            {
                if (c2.GetEdifice(map) == null)
                {
                    FilthMaker.TryMakeFilth(c2, map, ThingDefOf.Filth_Fuel, 1, FilthSourceFlags.None, true);
                }
            }
        }

        private void GenerateHive(Map map, IntVec3 cell)
        {
            (GenSpawn.Spawn(ThingDefOf.Hive, cell, map, WipeMode.Vanish) as Hive).GetComp<CompSpawnerHives>().canSpawnHives = false;
            int randomInRange = JellyCountRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                if (CellFinder.TryFindRandomCellNear(cell, map, SpawnRadius, c => c.Standable(map), out var loc))
                {
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.InsectJelly, null);
                    thing.stackCount = JellyStackCountRange.RandomInRange;
                    thing.SetForbidden(true, true);
                    GenSpawn.Spawn(thing, loc, map, WipeMode.Vanish);
                }
            }
            int randomInRange2 = GlowPodCountRange.RandomInRange;
            for (int j = 0; j < randomInRange2; j++)
            {
                if (CellFinder.TryFindRandomCellNear(cell, map, SpawnRadius, c => c.Standable(map), out var loc2))
                {
                    GenSpawn.Spawn(ThingDefOf.GlowPod, loc2, map, WipeMode.Vanish);
                }
            }
            foreach (IntVec3 c2 in GridShapeMaker.IrregularLump(cell, map, FilthAreaSize, null))
            {
                if (c2.GetEdifice(map) == null && Rand.Chance(FilthSpawnChance))
                {
                    FilthMaker.TryMakeFilth(c2, map, ThingDefOf.Filth_Slime, 1, FilthSourceFlags.None, true);
                }
            }
        }

        private void GenerateCorpseGear(Map map, IntVec3 cell)
        {
            List<ThingDef> source = new List<ThingDef>
            {
                ThingDefOf.MedicineIndustrial,
                ThingDefOf.MealSurvivalPack
            };
            Faction faction;
            Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out faction, true, false, TechLevel.Undefined, false);
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Drifter,
                                                                             faction,
                                                                             PawnGenerationContext.NonPlayer,
                                                                             -1,
                                                                             false,
                                                                             false,
                                                                             false,
                                                                             true,
                                                                             false,
                                                                             1f,
                                                                             false,
                                                                             true,
                                                                             false,
                                                                             true,
                                                                             true,
                                                                             false,
                                                                             false,
                                                                             false,
                                                                             false,
                                                                             0f,
                                                                             0f,
                                                                             null,
                                                                             1f,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             false,
                                                                             false,
                                                                             false,
                                                                             false,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             0f,
                                                                             DevelopmentalStage.Adult,
                                                                             null,
                                                                             null,
                                                                             null,
                                                                             false,
                                                                             false,
                                                                             false,
                                                                             -1,
                                                                             0,
                                                                             false));
            pawn.health.SetDead();
            Corpse corpse = pawn.MakeCorpse(null, null);
            corpse.Age = Mathf.RoundToInt((float)(CorpseAgeRangeDays.RandomInRange * 60000));
            corpse.GetComp<CompRottable>().RotProgress += (float)corpse.Age;
            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
            GenSpawn.Spawn(corpse, cell, map, WipeMode.Vanish);
            if (CellFinder.TryFindRandomCellNear(cell, map, GearDist, (IntVec3 c) => c.Standable(map), out var loc))
            {
                Thing thing = ThingMaker.MakeThing(source.RandomElement<ThingDef>(), null);
                thing.stackCount = GearStackCountRange.RandomInRange;
                GenSpawn.Spawn(thing, loc, map, WipeMode.Vanish);
            }
        }

        private void GenerateCorpsePile(Map map, IntVec3 cell)
        {
            int randomInRange = CorpseCountRange.RandomInRange;
            Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out var faction, true, false, TechLevel.Undefined, false);
            int age = Mathf.RoundToInt((float)(CorpseAgeRangeDays.RandomInRange * 60000));
            for (int i = 0; i < randomInRange; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Drifter, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false, false, false, -1, 0, false));
                pawn.Kill(null, null);
                pawn.Corpse.Age = age;
                pawn.Corpse.GetComp<CompRottable>().RotProgress += (float)pawn.Corpse.Age;
                if (CellFinder.TryFindRandomCellNear(cell, map, CorpseSpawnRadius, c => c.Standable(map), out var loc))
                {
                    GenSpawn.Spawn(pawn.Corpse, loc, map, WipeMode.Vanish);
                }
            }
        }

        private void GenerateSleepingFleshbeasts(Map map, IntVec3 cell)
        {
            int randomInRange = NumFleshbeastsRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Fingerspike, Faction.OfEntities);
                if (CellFinder.TryFindRandomCellNear(cell, map, SleepingFleshbeastSpawnRadius, c => c.Standable(map) && c.GetFirstPawn(map) == null, out var loc) && GenSpawn.Spawn(newThing, loc, map, WipeMode.Vanish).TryGetComp(out CompCanBeDormant compCanBeDormant))
                {
                    compCanBeDormant.ToSleep();
                }
            }
        }

        public int InterestPointSize = 20;
        public float MinDistApart = 10f;
        public float PatchDensity = 0.7f;
        public int ChemfuelPuddleSize = 20;
        public int ChemfuelStackMaxDist = 2;
        public int SpawnRadius = 3;
        public int FilthAreaSize = 20;
        public float FilthSpawnChance = 0.3f;
        public int GearDist = 1;
        public int CorpseSpawnRadius = 4;
        public int SleepingFleshbeastSpawnRadius = 4;
        public IntRange InterestPointCountRange = new IntRange(0, 3);




        public IntRange PatchSizeRange = new IntRange(50, 70);

        public IntRange ChemfuelCountRange = new IntRange(3, 5);

        public IntRange ChemfuelStackCountRange = new IntRange(10, 20);



        public IntRange JellyCountRange = new IntRange(2, 3);

        public IntRange JellyStackCountRange = new IntRange(15, 40);


        public IntRange GlowPodCountRange = new IntRange(1, 2);



        public IntRange CorpseAgeRangeDays = new IntRange(15, 120);


        public IntRange GearStackCountRange = new IntRange(2, 5);

        public IntRange CorpseCountRange = new IntRange(3, 6);


        public IntRange NumFleshbeastsRange = new IntRange(2, 4);


        private enum CaveInterestKind
        {
            MushroomPatch,
            ChemfuelGenerator,
            InsectHive,
            CorpseGear,
            CorpsePile,
            SleepingFleshbeasts
        }
    }
}
