using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(MapInfo), nameof(MapInfo.Tile), HarmonyLib.MethodType.Getter)]
    internal static class MapInfo_Tile_Patch
    {
        public static bool Prefix(MapInfo __instance,
#if v16
            ref PlanetTile __result
#else
            ref int __result
#endif
            )
        {
            if (__instance.isPocketMap && __instance.parent is PocketMapParent pocketMapParent && pocketMapParent.Map.GetComponent<CaveMapComponent>() != null)
            {
                __result = pocketMapParent.sourceMap.Tile;
                return false;
            }
            return true;
        }
    }
}
