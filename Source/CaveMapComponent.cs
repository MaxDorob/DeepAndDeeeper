using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Sound;
using Verse;
using UnityEngine;
using Mono.Unix.Native;

namespace Shashlichnik
{
    public class CaveMapComponent : UndercaveMapComponent
    {
        public List<EffecterDef> ambient = [DefsOf.ShashslichnikCaveCeilingDebris];
        public Map SourceMap
        {
            get
            {
                PocketMapParent pocketMapParent = map.Parent as PocketMapParent;
                if (pocketMapParent == null)
                {
                    return null;
                }
                return pocketMapParent.sourceMap;
            }
        }

        public CaveMapComponent(Map map) : base(map)
        {
        }


        public override void MapGenerated()
        {
            Map sourceMap = SourceMap;

            caveEntrance = sourceMap?.listerThings?.ThingsOfDef(DefsOf.CaveEntrance).FirstOrDefault() as CaveEntrance;
            caveExit = map.listerThings.ThingsOfDef(DefsOf.CaveExit).FirstOrDefault() as CaveExit;
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
                    Log.Message($"Recalculated count of rocks: {initialRockCount}");
                }
                return initialRockCount;
            }
        }
        private float stabilityPercentCached = 9999f;
        public float StabilityPercent
        {
            get
            {
                if (stabilityPercentCached >= 1000f)
                {
                    var countToCollapse = InitialRockCount / 5f;
                    var minedCount = InitialRockCount - CurrentRockCount;
                    var tickPassed = Find.TickManager.TicksGame - caveEntrance.tickOpened;
                    stabilityPercentCached = 1f - minedCount / countToCollapse - tickPassed / (GenDate.TicksPerDay * 12);
                }
                return stabilityPercentCached;
            }
        }

        public float LandslideChance => landslideChancePerStability.Evaluate(StabilityPercent);

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
            if (caveEntrance.IsCollapsing)
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
            if (StabilityPercent <= 0.01f)
            {
                caveEntrance.BeginCollapsing();
                return;
            }
            if (Rand.Chance(LandslideChance))
            {
                QueueLandslide(landslideTicksRange.RandomInRange, true);
            }
        }
        SimpleCurve HoursToShakeMTBTicksCurve = new SimpleCurve()
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
        private void ProcessCollapsing()
        {
            float mtb = HoursToShakeMTBTicksCurve.Evaluate(caveEntrance.TicksUntilCollapse / 2500f);
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
                    collapsingSustainer = SoundDefOf.UndercaveCollapsingStage2.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick)) ?? DefsOf.ShashlichnikVanillaCollapsingStage2.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerTick));
                }
            }
            if (Find.CurrentMap == map && Rand.MTBEventOccurs(mtb, 1f, 1f))
            {
                QueueLandslide(collapseTicksRange.RandomInRange, false);
            }
            collapsingSustainer.Maintain();
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

                    DefsOf.ShashslichnikCaveCeilingDebris.SpawnMaintained(landslide.Key, this.map, 1f);
                }
            }
        }
        public new void Notify_BeginCollapsing(int collapseDurationTicks)
        {
            SoundDefOf.UndercaveRumble.PlayOneShotOnCamera(map);
            Find.CameraDriver.shaker.DoShake(0.2f, 120);
        }

        public static IntRange landslideTicksRange = new IntRange(GenDate.TicksPerHour, GenDate.TicksPerHour * 2);
        public static IntRange collapseTicksRange = new IntRange(GenDate.TicksPerHour / 10, GenDate.TicksPerHour);
        public void QueueLandslide(int ticks, bool sendMessage)
        {
            if (CellFinderLoose.TryGetRandomCellWith(c => (c.GetEdifice(map) == null || !IsRock(c)) && !queuedLandslides.ContainsKey(c), map, 1000, out var cell))
            {
                queuedLandslides.Add(cell, ticks);
                if (sendMessage)
                {
                    Messages.Message("LandslideIncoming".Translate(), new LookTargets(cell, map), MessageTypeDefOf.ThreatSmall, false);
                }
            }
            else
            {
                Log.Warning("Can't find a cell for landslide");
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
        }

        public Dictionary<IntVec3, int> queuedLandslides = new Dictionary<IntVec3, int>();
        public CaveEntrance caveEntrance;
        public CaveExit caveExit;
        public int initialRockCount;
    }
}
