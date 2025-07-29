﻿using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Sound;
using Verse;
using UnityEngine;


namespace Shashlichnik
{
    public class CaveMapComponent : UndercaveMapComponent
    {
        internal static Dictionary<Map, CaveMapComponent> cachedComponents = new Dictionary<Map, CaveMapComponent>();
        public List<EffecterDef> ambient = [DefsOf.ShashlichnikCaveCeilingDebris];
        public CaveMapComponent(Map map) : base(map)
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            cachedComponents.Add(this.map, this);
        }
        public override void MapRemoved()
        {
            base.MapRemoved();
            cachedComponents.Remove(this.map);
        }
        public override void MapGenerated()
        {
            Map sourceMap = SourceMap;

            caveEntrance = sourceMap?.listerThings?.ThingsOfDef(DefsOf.ShashlichnikCaveEntrance).FirstOrDefault() as CaveEntrance;
            caveExit = map.listerThings.ThingsOfDef(DefsOf.ShashlichnikCaveExit).FirstOrDefault() as CaveExit;
            if (caveEntrance == null)
            {
                Log.Warning("Cave entrance was not found after generating cave, if this map was created via dev tools you can ignore this");
                return;
            }
            if (caveExit == null)
            {
                Log.Error("Cave exit was not found after generating cave");
                return;
            }
        }

        public int InitialRockCount
        {
            get
            {
                if (initialRockCount <= 0)
                {
                    initialRockCount = CurrentRockCount;
                }
                return initialRockCount;
            }
        }
        private float stabilityPercentCached = 9999f;
        public float StabilityPercent
        {
            get
            {
                if (!Mod.Settings.stabilitySystemEnabled)
                {
                    return 1f;
                }
                if (stabilityPercentCached >= 1000f)
                {
                    var countToCollapse = InitialRockCount / 5f;
                    var minedCount = InitialRockCount - CurrentRockCount;
                    var tickPassed = Find.TickManager.TicksGame - caveEntrance.tickOpened;
                    var buildingsImpact = BuildingsImpact;
                    stabilityPercentCached = Mathf.Min(1f + buildingsImpact - minedCount / countToCollapse - tickPassed / (GenDate.TicksPerDay * 12f), 1.0f);
                }
                return stabilityPercentCached;
            }
        }
        public IEnumerable<Building> StabilityBuildings
        {
            get
            {
                var stabilizers = map.listerBuildings.AllColonistBuildingsOfType<Building>().Where(x => x.def.HasModExtension<DefModExtension_CaveStabilizer>()).ToList();
                return stabilizers;
            }
        }
        public IEnumerable<IntVec3> StabilizedCells => StabilityBuildings.SelectMany(b => GenRadial.RadialCellsAround(b.Position, b.def.GetModExtension<DefModExtension_CaveStabilizer>().effectiveRadius, true)).Distinct();
        public float BuildingsImpact => StabilizedCells.Count() * 0.00065f;
        public float LandslideChance => landslideChancePerStability.Evaluate(StabilityPercent);

        public int WastepackPoints
        {
            get
            {
                if (!ModsConfig.BiotechActive)
                {
                    return 0;
                }
                int result = map.listerThings.ThingsOfDef(ThingDefOf.Wastepack).Count;
                result += map.pollutionGrid.grid.TrueCount;
                return result;
            }
        }

        public override void MapComponentTick()
        {
            if (map.IsHashIntervalTick(GenDate.TicksPerHour))
            {
                stabilityPercentCached = 9999f;
            }
            if (Find.CurrentMap != map)
            {
                Sustainer sustainer = collapsingSustainer;
                if (sustainer != null)
                {
                    sustainer.End();
                }
            }
            if (IsCollapsing)
            {
                ProcessCollapsing();
            }
            else
            {
                ProcessStability();
            }

            if (caveEntrance != null && Find.CurrentMap == map)
            {
                ProcessAmbient();
            }
            ProcessLandslide();
        }
        SimpleCurve landslideChancePerStability = new SimpleCurve()
        {
            {new CurvePoint(1.0f, 1f / GenDate.TicksPerDay), true }, // 1 landslide per day
            {new CurvePoint(0.5f, 4f / GenDate.TicksPerDay), true }, // ~4 per day
            {new CurvePoint(0.2f, 12f / GenDate.TicksPerDay), true }, // ~12 per day
            {new CurvePoint(0.0f, 1f / GenDate.TicksPerHour), true } // 24 per day
        };
        private void ProcessStability()
        {
            if (!Mod.Settings.stabilitySystemEnabled)
            {
                return;
            }
            if (StabilityPercent <= 0.01f)
            {
                caveEntrance.BeginCollapsing();
                return;
            }
            if (Mod.Settings.landslidesEnabled && Rand.Chance(LandslideChance))
            {
                QueueLandslide(landslideTicksRange.RandomInRange, true);
            }
        }
        new SimpleCurve HoursToShakeMTBTicksCurve = new SimpleCurve()
        {
            {
                new CurvePoint(14f, 500f),
                true
            },
            {
                new CurvePoint(1f, 12f),
                true
            }
        };
        public bool IsCollapsing => collapseTick.HasValue;
        public int TicksUntilCollapse
        {
            get
            {
                return this.collapseTick.Value - Find.TickManager.TicksGame;
            }
        }
        private void ProcessCollapsing()
        {
            if (TicksUntilCollapse <= 0)
            {
                FinishCollapsing();
                return;
            }
            if (Gen.IsHashIntervalTick(map, 1800) && map.mapPawns.FreeColonistsAndPrisonersSpawnedCount <= 0 && !this.map.listerThings.GetThingsOfType<CaveExit>().Any(x => x.caveEntrance != null))
            {
                FinishCollapsing();
                return;
            }

            float mtb = HoursToShakeMTBTicksCurve.Evaluate(TicksUntilCollapse / 2500f);
            if (map == Find.CurrentMap)
            {
                if (caveEntrance.CollapseStage == 1)
                {
                    if (collapsingSustainer == null || collapsingSustainer.Ended)
                    {
                        collapsingSustainer = SoundDefOf.UndercaveCollapsingStage1?.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick)) ?? DefsOf.ShashlichnikVanillaCollapsingStage1.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick));
                    }
                }
                else
                {
                    if (collapsingSustainer == null || collapsingSustainer.Ended)
                    {
                        collapsingSustainer = SoundDefOf.UndercaveCollapsingStage2?.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick)) ?? DefsOf.ShashlichnikVanillaCollapsingStage2.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick));
                    }
                }
                collapsingSustainer.Maintain();
            }
            if (Find.CurrentMap == map && Rand.MTBEventOccurs(mtb, 1f, 1f))
            {
                QueueLandslide(collapseTicksRange.RandomInRange, false);
            }
        }
        private void ProcessAmbient()
        {
            if (Rand.MTBEventOccurs(60f, 1f, 1f))
            {
                IntVec3 effectCell;
                if (Mod.Settings.AnomalyEffectsEnabled && Rand.Chance(0.3f) && Find.CameraDriver.ZoomRootSize < 20f && CellFinderLoose.TryGetRandomCellWith((IntVec3 c) => c.Standable(this.map) && Find.CameraDriver.CurrentViewRect.Contains(c), this.map, 100, out effectCell))
                {
                    EffecterDefOf.UndercaveMapAmbienceWater.SpawnMaintained(effectCell, map);
                }
                else if (CellFinderLoose.TryGetRandomCellWith((IntVec3 c) => c.Standable(map), map, 100, out effectCell))
                {
                    if (effectCell.Fogged(this.map))
                    {
                        return;
                    }
                    if (!Find.CameraDriver.CurrentViewRect.Contains(effectCell))
                    {
                        return;
                    }
                    ambient.RandomElementByWeight((EffecterDef p) => p.randomWeight).SpawnMaintained(effectCell, map);
                }
            }
        }
        private SimpleCurve landslideEffecterChance = new SimpleCurve()
        {
            {new CurvePoint(0f, 0.03f), true },
            {new CurvePoint(100f, 0.01f), true },
            {new CurvePoint(3000f, 0.002f), true }
        };
        private void ProcessLandslide()
        {
            if (map?.Disposed ?? true)
            {
                return;
            }
            foreach (var landslide in queuedLandslides.ToList())
            {
                queuedLandslides[landslide.Key]--;
                if (queuedLandslides[landslide.Key] <= 0)
                {
                    RoofCollapserImmediate.DropRoofInCells(landslide.Key, this.map, null);
                    queuedLandslides.Remove(landslide.Key);
                }
                else if (Rand.Chance(landslideEffecterChance.Evaluate(queuedLandslides[landslide.Key])))
                {

                    DefsOf.ShashlichnikCaveCeilingDebris.SpawnMaintained(landslide.Key, this.map, 1f);
                }
            }
        }
        public void BeginCollapsing(CaveEntrance sender, bool silent = false, int? forcedCollapseTick = null)
        {
            if (IsCollapsing)
            {
                return;
            }
            collapseTick = forcedCollapseTick ?? Find.TickManager.TicksGame + CollapseDurationTicks.RandomInRange;
            foreach (var caveEntrance in map.listerThings.GetThingsOfType<CaveEntrance>())
            {
                caveEntrance.BeginCollapsing(true);
            }
            if (silent)
            {
                return;
            }
            SoundDefOf.UndercaveRumble?.PlayOneShotOnCamera(map);
            Find.CameraDriver.shaker.DoShake(0.2f, 120);
            var letter = LetterMaker.MakeLetter("ShashlichnikCaveCollapsing".Translate(), "ShashlichnikCaveCollapsingDesc".Translate(), LetterDefOf.ThreatBig, new LookTargets(caveExit));
            Find.LetterStack.ReceiveLetter(letter);
        }
        public void Notify_ExitDestroyed(CaveExit caveExit, IntVec3 cell)
        {
            if (this.map.listerThings.GetThingsOfType<CaveExit>().Except(caveExit).Any() && map.mapPawns.FreeColonistsAndPrisonersCount > 0)
            {
                Messages.Message("ShashlichnikMessageCaveExitCollapsed".Translate(), new TargetInfo(cell, map, false), MessageTypeDefOf.NeutralEvent, true);
                QueueSingleLandslide(cell, Rand.Range(10, 120));
                foreach (var landslideCell in GenAdj.CellsAdjacentCardinal(cell, Rot4.North, IntVec2.One))
                {
                    QueueSingleLandslide(landslideCell, Rand.Range(10, 120));
                }
            }
            else
            {
                FinishCollapsing();
            }
        }
        public static SimpleCurve infestationScaleCurve = new SimpleCurve()
        {
            new CurvePoint(0.0f, 0.0f),
            new CurvePoint(59.9f, 0.0f),
            new CurvePoint(60f, 0.8f),
            new CurvePoint(300f, 1.5f),
            new CurvePoint(800f, 2.5f),
            new CurvePoint(10000f, 2.5f)
        };
        protected void FinishCollapsing()
        {
            DamageInfo damageInfo = new DamageInfo(DamageDefOf.Crush, 99999f, 999f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true, QualityCategory.Normal, true);
            for (int i = map.mapPawns.AllPawns.Count - 1; i >= 0; i--)
            {
                Pawn pawn = map.mapPawns.AllPawns[i];
                pawn.TakeDamage(damageInfo);
                if (!pawn.Dead)
                {
                    pawn.Kill(new DamageInfo?(damageInfo), null);
                }
            }
            Messages.Message("ShashlichnikMessageCaveCollapsed".Translate(), new TargetInfo(caveExit.caveEntrance?.Position ?? IntVec3.Invalid, caveExit.caveEntrance?.Map, false), MessageTypeDefOf.NeutralEvent, true);
            var infestationScale = infestationScaleCurve.Evaluate(WastepackPoints);
            if (caveEntrance.Level == 0 && infestationScale > 0.5f)
            {
                StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.FirstOrDefault((StorytellerComp x) => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
                if (storytellerComp != null)
                {
                    IncidentParms parms = storytellerComp.GenerateParms(IncidentCategoryDefOf.ThreatBig, this.SourceMap);
                    parms.points *= infestationScale;
                    parms.forced = true;
                    parms.infestationLocOverride = caveEntrance.Position;
                    var incidentDef = IncidentDefOf.Infestation;
                    incidentDef.Worker.TryExecute(parms);
                }
                else
                {
                    Log.Warning($"storytellerComp was null for some reason. Storyteller: {Find.Storyteller.def?.defName}");
                }
            }
            foreach (var caveExit in map.listerThings.GetThingsOfType<CaveExit>())
            {
                if (!caveExit.Destroyed)
                {
                    caveExit.Destroy(DestroyMode.KillFinalize);
                }
            }
            PocketMapUtility.DestroyPocketMap(map);
        }
        public static IntRange landslideTicksRange = new IntRange(GenDate.TicksPerHour, GenDate.TicksPerHour * 2);
        public static IntRange collapseTicksRange = new IntRange(GenDate.TicksPerHour / 10, GenDate.TicksPerHour);
        public void QueueLandslide(int ticks, bool sendMessage)
        {
            if (CellFinderLoose.TryGetRandomCellWith(c => (c.GetEdifice(map) == null || !IsRock(c)) && !queuedLandslides.ContainsKey(c), map, 1000, out var cell))
            {
                QueueSingleLandslide(cell, ticks);
                if (sendMessage && !cell.Fogged(map))
                {
                    Messages.Message("ShashlichnikLandslideIncoming".Translate(), new LookTargets(cell, map), MessageTypeDefOf.ThreatSmall, false);
                }
                int subLandslidesCount = Rand.Range(0, 4);
                for (int i = 0; i < subLandslidesCount; i++)
                {
                    var subLandslideCell = GenAdj.RandomAdjacentCell8Way(cell);
                    if (subLandslideCell.InBounds(map) && !queuedLandslides.ContainsKey(subLandslideCell) && (subLandslideCell.GetEdifice(map)?.def.building.isNaturalRock ?? true))
                    {
                        QueueSingleLandslide(subLandslideCell, ticks + Rand.Range(-120, 120));
                    }

                }
            }
            else
            {
                Log.Warning("Can't find a cell for landslide");
            }
        }
        public void QueueSingleLandslide(IntVec3 cell, int ticks)
        {
            if (!queuedLandslides.ContainsKey(cell))
            {
                queuedLandslides.Add(cell, ticks);
            }
        }


        public int CurrentRockCount
        {
            get
            {
                return map.edificeGrid.InnerArray.Where(IsRock).Count();
            }
        }
        private bool IsRock(IntVec3 cell)
        {
            return IsRock(cell.GetEdifice(map));
        }
        private bool IsRock(Building building)
        {
            if (building == null)
            {
                return false;
            }
            return building.def.building.isNaturalRock;
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref initialRockCount, nameof(initialRockCount));
            Scribe_References.Look(ref caveEntrance, nameof(caveEntrance), false);
            Scribe_References.Look(ref caveExit, nameof(caveExit), false);
            Scribe_Collections.Look(ref queuedLandslides, nameof(queuedLandslides), LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref collapseTick, nameof(collapseTick), null);

        }

        private static readonly IntRange CollapseDurationTicks = new IntRange(GenDate.TicksPerHour * 3, GenDate.TicksPerHour * 7);
        public Dictionary<IntVec3, int> queuedLandslides = new Dictionary<IntVec3, int>();
        public CaveEntrance caveEntrance;
        public CaveExit caveExit;
        public int initialRockCount;
        public int? collapseTick;
    }
}
