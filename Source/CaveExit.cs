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

        public override Texture2D EnterTex
        {
            get
            {
                return CaveExit.ExitCaveTex.Texture;
            }
        }

        public override Map GetOtherMap()
        {
            return caveEntrance.Map;
        }

        public override IntVec3 GetDestinationLocation()
        {
            return caveEntrance.Position;
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
        }

        private static readonly CachedTexture ExitCaveTex = new CachedTexture("UI/Overlays/Arrow");

        private static readonly CachedTexture ViewSurfaceTex = new CachedTexture("UI/Widgets/Search");

        private static readonly Vector3 RopeDrawOffset = new Vector3(0f, 1f, 1f);

        public CaveEntrance caveEntrance;
        public bool exitIfNoJob = false;
    }
}
