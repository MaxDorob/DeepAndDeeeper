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


namespace Shashlichnik
{
    public class CaveMapComponent : UndercaveMapComponent
    {
        public List<EffecterDef> ambient = [DefsOf.ShashlichnikCaveCeilingDebris];
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
        public void Notify_BeginCollapsing(CaveEntrance sender, int collapseDurationTicks)
        {
            foreach (var otherExit in this.map.listerThings.GetThingsOfType<CaveExit>())
            {
                otherExit.caveEntrance?.BeginCollapsing(collapseDurationTicks, false);
            }
            SoundDefOf.UndercaveRumble?.PlayOneShotOnCamera(map);
            Find.CameraDriver.shaker.DoShake(0.2f, 120);
            var letter = LetterMaker.MakeLetter("ShashlichnikCaveCollapsing".Translate(), "ShashlichnikCaveCollapsingDesc".Translate(), LetterDefOf.ThreatBig, new LookTargets(caveExit));
            Find.LetterStack.ReceiveLetter(letter);
        }
        public void Notify_ExitDestroyed(CaveEntrance sender)
        {
            if (this.map.listerThings.GetThingsOfType<CaveExit>().Except(sender.caveExit).Any())
            {
                var cell = sender.caveExit.Position;
                Messages.Message("ShashlichnikMessageCaveExitCollapsed".Translate(), new TargetInfo(cell, map, false), MessageTypeDefOf.NeutralEvent, true);
                Thing.allowDestroyNonDestroyable = true;
                sender.caveExit.Destroy(DestroyMode.Vanish);
                Thing.allowDestroyNonDestroyable = false;
                QueueSingleLandslide(cell, Rand.Range(10, 120));
                foreach (var landslideCell in GenAdj.CellsAdjacentCardinal(cell, Rot4.North, IntVec2.One))
                {
                    QueueSingleLandslide(landslideCell, Rand.Range(10, 120));
                }
            }
            else
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
                PocketMapUtility.DestroyPocketMap(map);
                Messages.Message("ShashlichnikMessageCaveCollapsed".Translate(), new TargetInfo(sender.Position, sender.Map, false), MessageTypeDefOf.NeutralEvent, true);
            }
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
        }

        public Dictionary<IntVec3, int> queuedLandslides = new Dictionary<IntVec3, int>();
        public CaveEntrance caveEntrance;
        public CaveExit caveExit;
        public int initialRockCount;
    }
}
