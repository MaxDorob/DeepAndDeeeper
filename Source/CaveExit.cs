using RimWorld;
using Shashlichnik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Shashlichnik
{
    public class CaveExit :
#if v16
        PocketMapExit
#else
        MapPortal
#endif
    {
#if v16
        public override string EnterString
#else
        public override string EnterCommandString
#endif
        {
            get
            {
                return "ShashlichnikExitCave".Translate();
            }
        }
        public bool WayToSurface
        {
            get
            {
                if (wayToSurface == null)
                {
                    wayToSurface = caveEntrance?.Map.GetComponent<CaveMapComponent>() == null;
                }
                return wayToSurface.Value;
            }
            set
            {
                wayToSurface = value;
            }
        }
        public override Texture2D EnterTex
        {
            get
            {
                return CaveExit.ExitCaveTex.Texture;
            }
        }

        public override Map GetOtherMap()
        {
            if (caveEntrance == null)
            {
                var mapSize = Mod.Settings.mapSize;
                PocketMapUtility.currentlyGeneratingPortal = this;
                var caveComp = base.Map.GetComponent<CaveMapComponent>();
                var level = caveComp?.Level + 1 ?? 0;
                var map = PocketMapUtility.GeneratePocketMap(new IntVec3(mapSize, 1, mapSize), DefsOf.ShashlichnikScenarioUndergroundLvl2, null, base.Map);
                caveEntrance = map.listerThings.ThingsOfDef(DefsOf.ShashlichnikCaveEntrance).Last() as CaveEntrance;
                caveEntrance.caveExit = this;
                caveEntrance.cave = map;
                caveEntrance.TicksToOpen = 0;
                PocketMapUtility.currentlyGeneratingPortal = null;
            }
            return caveEntrance.Map;
        }

        public override IntVec3 GetDestinationLocation()
        {
            return caveEntrance.Position;
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            if (!respawningAfterLoad && PocketMapUtility.currentlyGeneratingPortal == null)
            {
                PocketMapUtility.currentlyGeneratingPortal = this;
            }
            base.SpawnSetup(map, respawningAfterLoad);
            if (PocketMapUtility.currentlyGeneratingPortal == this)
            {
                PocketMapUtility.currentlyGeneratingPortal = null;
            }
        }
        public override void OnEntered(Pawn pawn)
        {   
            base.OnEntered(pawn);
            if (Find.CurrentMap == base.Map)
            {
                SoundDefOf.TraversePitGate?.PlayOneShot(this);
                return;
            }
            if (Find.CurrentMap == caveEntrance.Map)
            {
                SoundDefOf.TraversePitGate?.PlayOneShot(caveEntrance);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            yield return new Command_Toggle
            {
                defaultLabel = "ShashlichnikExitIfNoWork".Translate(),
                defaultDesc = "ShashlichnikExitIfNoWorkDesc".Translate(),
                icon = CaveExit.ExitCaveTex.Texture,
                isActive = () => exitIfNoJob,
                toggleAction = () => exitIfNoJob = !exitIfNoJob
            };
            yield return new Command_Action
            {
                defaultLabel = "ViewSurface".Translate(),
                defaultDesc = "ViewSurfaceDesc".Translate(),
                icon = CaveExit.ViewSurfaceTex.Texture,
                action = delegate ()
                {
                    CameraJumper.TryJumpAndSelect(caveEntrance, CameraJumper.MovementMode.Pan);
                }
            };
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            var cell = Position;
            Thing.allowDestroyNonDestroyable = true;
            base.Destroy(mode);
            Thing.allowDestroyNonDestroyable = false;
            if (Map != null && !Map.Disposed)
            {
                Map.GetComponent<CaveMapComponent>().Notify_ExitDestroyed(this, cell);
            }
            if (caveEntrance != null && !caveEntrance.Destroyed)
            {
                caveEntrance.Destroy(mode);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref caveEntrance, nameof(caveEntrance), false);
            Scribe_Values.Look(ref exitIfNoJob, nameof(exitIfNoJob), false);
            Scribe_Values.Look(ref wayToSurface, nameof(wayToSurface), null);
        }

        private static readonly CachedTexture ExitCaveTex = new CachedTexture("UI/Overlays/Arrow");

        private static readonly CachedTexture ViewSurfaceTex = new CachedTexture("UI/Widgets/Search");

        private static readonly Vector3 RopeDrawOffset = new Vector3(0f, 1f, 1f);

        public CaveEntrance caveEntrance;
        public bool exitIfNoJob = false;
        private bool? wayToSurface;
    }
}
