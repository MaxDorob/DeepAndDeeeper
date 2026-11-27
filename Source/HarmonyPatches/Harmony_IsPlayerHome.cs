using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(Map), nameof(Map.IsPlayerHome), HarmonyLib.MethodType.Getter)]
    internal class Harmony_IsPlayerHome
    {
        public static void Postfix(Map __instance, ref bool __result)
        {
            if (__result && CaveMapComponent.cachedComponents.ContainsKey(__instance))
            {
                __result = false;
            }
        }
    }
}
