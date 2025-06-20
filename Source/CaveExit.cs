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
    public class CaveExit : MapPortal
    {
        public override string EnterCommandString
        {
            get
            {
                return "ExitCave".Translate();
            }
        }

        public override Texture2D EnterTex
        {
            get
            {
                return CaveExit.ExitCaveTex.Texture;
            }
        }

        private Graphic RopeGraphic
        {
            get
            {
                if (cachedRopeGraphic == null)
                {
                    if (ModsConfig.AnomalyActive)
                    {
                        cachedRopeGraphic = GraphicDatabase.Get<Graphic_Single_AgeSecs>("Things/Building/PitGate/PitGateExit/PitGateExit_Rope", ShaderDatabase.CaveExitRope, def.graphicData.drawSize, Color.white);
                    }
                    else
                    {
                        cachedRopeGraphic = GraphicDatabase.Get<Graphic_Single>("Things/Building/Misc/MarriageSpot");
                    }
                }
                return cachedRopeGraphic;
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

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            RopeGraphic.Draw(DrawPos + CaveExit.RopeDrawOffset, Rot4.North, this, 0f);
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref caveEntrance, nameof(caveEntrance), false);
        }

        private static readonly CachedTexture ExitCaveTex = new CachedTexture("UI/Overlays/Arrow");

        private static readonly CachedTexture ViewSurfaceTex = new CachedTexture("UI/Widgets/Search");

        private static readonly Vector3 RopeDrawOffset = new Vector3(0f, 1f, 1f);

        public CaveEntrance caveEntrance;

        [Unsaved(false)]
        private Graphic cachedRopeGraphic;
    }
}
