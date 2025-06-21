using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    
    internal static class MapTemperature_Patch
    {
        [HarmonyLib.HarmonyPatch(typeof(MapTemperature), nameof(MapTemperature.OutdoorTemp), HarmonyLib.MethodType.Getter)]
        internal static class OutdoorTemperature_Patch
        {
            public static bool Prefix(MapTemperature __instance, ref float __result)
            {
                var caveComp = __instance.map.GetComponent<CaveMapComponent>();
                if (caveComp?.SourceMap != null )
                {
                    __result = caveComp.SourceMap.mapTemperature.OutdoorTemp * Mod.Settings.undergroundTemperatureModifier;
                    return false;
                }
                return true;
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(MapTemperature), nameof(MapTemperature.SeasonalTemp), HarmonyLib.MethodType.Getter)]
        internal static class SeasonalTemperature_Patch
        {
            public static bool Prefix(MapTemperature __instance, ref float __result)
            {
                var caveComp = __instance.map.GetComponent<CaveMapComponent>();
                if (caveComp?.SourceMap != null)
                {
                    __result = caveComp.SourceMap.mapTemperature.SeasonalTemp * Mod.Settings.undergroundTemperatureModifier;
                    return false;
                }
                return true;
            }
        }
    }
}
