using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.Sound;
using Verse;
using Shashlichnik;

namespace Shashlichnik
{
    [StaticConstructorOnStartup]
    public class CaveEntrance : MapPortal
    {
        private Graphic graphicWithRope;
        public override Graphic Graphic
        {
            get
            {
                if (ticksToOpen > tickToOpenConst * 0.7f)
                {
                    return base.Graphic;
                }
                if (graphicWithRope == null)
                {
                    var baseGraphic = base.Graphic;
                    graphicWithRope = GraphicDatabase.Get<Graphic_Single>("Things/Buildings/CavernHoleRope", baseGraphic.Shader, baseGraphic.drawSize, baseGraphic.Color, baseGraphic.ColorTwo, def.graphicData, baseGraphic.maskPath);
                }
                return graphicWithRope;
            }
        }
        public bool IsCollapsing
        {
            get
            {
                return isCollapsing;
            }
        }

        public int CollapseStage
        {
            get
            {
                if (collapseTick - Find.TickManager.TicksGame >= 3600)
                {
                    return 1;
                }
                return 2;
            }
        }

        public int TicksUntilCollapse
        {
            get
            {
                return collapseTick - Find.TickManager.TicksGame;
            }
        }

        public override Texture2D EnterTex
        {
            get
            {
                return enterTex;
            }
        }

        public override string EnterCommandString
        {
            get
            {
                return "ShashlichnikEnterCave".Translate();
            }
        }
        public int TicksToOpen
        {
            get
            {
                return ticksToOpen;
            }
            set
            {
                if (ticksToOpen <= 0)
                {
                    return;
                }
                ticksToOpen = value;
                tickOpened = Find.TickManager.TicksGame;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickOpened, nameof(tickOpened), 0, false);
            Scribe_Values.Look(ref collapseTick, nameof(collapseTick), 0, false);
            Scribe_Values.Look(ref isCollapsing, nameof(isCollapsing), false, false);
            Scribe_References.Look(ref cave, nameof(cave), false);
            Scribe_References.Look(ref caveExit, nameof(caveExit), false);
            Scribe_Values.Look(ref beenEntered, nameof(beenEntered), false, false);
            Scribe_Values.Look(ref ticksToOpen, nameof(ticksToOpen));
        }

        public override void Tick()
        {
            base.Tick();
            if (IsCollapsing)
            {
                if (CollapseStage == 1)
                {
                    if (collapseEffecter1 == null)
                    {
                        collapseEffecter1 = DefsOf.ShashlichnikCaveEntranceCollapseStage1?.Spawn(this, base.Map, 1f);
                    }
                }
                else if (CollapseStage == 2)
                {
                    if (collapseSustainer == null && Mod.Settings.AnomalyEffectsEnabled)
                    {
                        collapseSustainer = SoundDefOf.PitGateCollapsing?.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
                    }
                    collapseSustainer?.Maintain();
                    if (collapseEffecter2 == null)
                    {
                        collapseEffecter2 = DefsOf.ShashlichnikCaveEntranceCollapseStage2?.Spawn(this, base.Map, 1f);
                    }
                    if (Find.CurrentMap == base.Map && Rand.MTBEventOccurs(2f, 60f, 1f))
                    {
                        Find.CameraDriver.shaker.DoShake(0.2f);
                    }
                }
                Effecter effecter = collapseEffecter1;
                if (effecter != null)
                {
                    effecter.EffectTick(this, this);
                }
                Effecter effecter2 = collapseEffecter2;
                if (effecter2 != null)
                {
                    effecter2.EffectTick(this, this);
                }
                if (Find.TickManager.TicksGame >= collapseTick)
                {
                    Destroy(DestroyMode.KillFinalize);
                }
                return;
            }

        }
        private void Collapse()
        {
            collapseSustainer?.End();
            Effecter effecter = collapseEffecter2;
            if (effecter != null)
            {
                effecter.Cleanup();
            }
            collapseEffecter2 = null;
            Effecter effecter2 = collapseEffecter1;
            if (effecter2 != null)
            {
                effecter2.Cleanup();
            }
            collapseEffecter1 = null;
            if (Mod.Settings.AnomalyEffectsEnabled)
            {
                EffecterDefOf.PitGateAboveGroundCollapsed?.Spawn(base.Position, base.Map, 1f);
            }

            if (cave != null && !cave.Disposed)
            {
                cave.GetComponent<CaveMapComponent>().Notify_ExitDestroyed(this);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode == DestroyMode.Vanish)
            {
                base.Destroy(mode);
                return;
            }
            Map map = base.Map;
            Collapse();
            base.Destroy(mode);
            EffecterDefOf.ImpactDustCloud?.Spawn(base.Position, map, 1f).Cleanup();
        }


        public void BeginCollapsing()
        {
            if (!isCollapsing)
            {
                BeginCollapsing(CaveEntrance.CollapseDurationTicks.RandomInRange);
            }
        }
        public void BeginCollapsing(int randomInRange, bool notify = true)
        {
            if (isCollapsing)
            {
                return;
            }
            randomInRange *= 1 - ticksToOpen / tickToOpenConst;
            collapseTick = Find.TickManager.TicksGame + randomInRange;
            Map map = cave;
            isCollapsing = true;
            if (notify && map != null)
            {
                map.GetComponent<CaveMapComponent>().Notify_BeginCollapsing(this, randomInRange);
            }
        }



        public void GenerateUndercave()
        {
            var mapSize = Mod.Settings.mapSize;
            cave = PocketMapUtility.GeneratePocketMap(new IntVec3(mapSize, 1, mapSize), DefsOf.ShashlichnikUnderground, null, base.Map);
            caveExit = cave.listerThings.ThingsOfDef(DefsOf.ShashlichnikCaveExit).First() as CaveExit;
            caveExit.caveEntrance = this;
        }

        public override bool IsEnterable(out string reason)
        {
            if (isCollapsing && !beenEntered)
            {
                reason = "ShashlichnikCaveCollapsing".Translate();
                return false;
            }
            if (TicksToOpen > 0)
            {
                reason = "CaveNotDugYet".Translate();
                return false;
            }

            reason = "";
            return true;
        }

        public override Map GetOtherMap()
        {
            if (cave == null)
            {
                GenerateUndercave();
            }
            return cave;
        }

        public override IntVec3 GetDestinationLocation()
        {
            var caveExit = this.caveExit;
            if (caveExit == null)
            {
                return IntVec3.Invalid;
            }
            return caveExit.Position;
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            var collapseChance = 2 * (MaxHitPoints - HitPoints) / MaxHitPoints;
            if (Rand.Chance(collapseChance))
            {
                BeginCollapsing();
            }
        }

        public override void OnEntered(Pawn pawn)
        {
            base.OnEntered(pawn);
            if (!beenEntered)
            {
                beenEntered = true;
            }
            if (Find.CurrentMap == base.Map)
            {
                SoundDefOf.TraversePitGate?.PlayOneShot(this);
                return;
            }
            if (Find.CurrentMap == caveExit.Map)
            {
                SoundDefOf.TraversePitGate?.PlayOneShot(caveExit);
            }
        }
        private static Texture2D enterTex = ContentFinder<Texture2D>.Get("UI/Buttons/Drop");
        private static Texture2D viewCaveTex = ContentFinder<Texture2D>.Get("UI/Widgets/Search");
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (cave != null)
            {
                yield return new Command_Action
                {
                    defaultLabel = "ViewUndercave".Translate(),
                    defaultDesc = "ViewUndercaveDesc".Translate(),
                    icon = viewCaveTex,
                    action = delegate ()
                    {
                        CameraJumper.TryJumpAndSelect(caveExit, CameraJumper.MovementMode.Pan);
                    }
                };
            }
            if (ticksToOpen > 0 && DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Finish digging",
                    action = () => TicksToOpen = 0
                };
            }
            if (isCollapsing)
            {
                yield break;
            }
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            yield return new Command_Action
            {
                defaultLabel = "DEV: Collapse cave",
                action = new Action(() => BeginCollapsing())
            };
        }


        private static readonly IntRange CollapseDurationTicks = new IntRange(GenDate.TicksPerHour * 3, GenDate.TicksPerHour * 7);
        public int tickOpened = -999999;
        public int collapseTick = -999999;
        private int ticksToOpen = tickToOpenConst;
        public const int tickToOpenConst = GenDate.TicksPerHour * 36;
        private bool isCollapsing;
        public Map cave;
        public CaveExit caveExit;
        internal bool beenEntered;
        internal Sustainer collapseSustainer;
        internal Effecter collapseEffecter1;
        internal Effecter collapseEffecter2;
    }
}