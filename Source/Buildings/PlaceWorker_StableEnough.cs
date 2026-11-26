using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace Shashlichnik
{
    [StaticConstructorOnStartup]
    public class PlaceWorker_StableEnough : PlaceWorker
    {
        static PlaceWorker_StableEnough()
        {
            if (material == null)
            {
                var color = Color.red;
                color.a = 0.5f;
                material ??= SolidColorMaterials.SimpleSolidColorMaterial(color);
            }
        }
        private static Material material;
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);
            foreach (var blockedCell in Find.CurrentMap.GetComponent<CaveEntranceTracker>().BlockedCells)
            {
                Graphics.DrawMesh(MeshPool.plane10, blockedCell.ToVector3Shifted().WithY(AltitudeLayer.MapDataOverlay.AltitudeFor()), Quaternion.identity, material, 0);
            }
        }
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (map.GetComponent<CaveEntranceTracker>().BlockedCells.Contains(loc))
                return new AcceptanceReport("ShashlichnikNotStableYet".Translate());
            return true;
        }
    }
}
