using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(Settlement), nameof(Settlement.IncidentTargetTags))]
    internal static class Harmony_ModifyCaveIncidentTarget
    {
        public static IEnumerable<IncidentTargetTagDef> Postfix(IEnumerable<IncidentTargetTagDef> values, Settlement __instance)
        {
            var isCave = __instance.Map?.GetComponent<CaveMapComponent>() != null;
            foreach (var value in values)
            {
                if (isCave && value == IncidentTargetTagDefOf.Map_PlayerHome)
                {
                    continue;
                }
                yield return value;
            }
            if (isCave)
            {
                yield return IncidentTargetTagDefOf.Map_Misc;
                yield return DefsOf.ShashlichnikMap_Cave;
            }
        }
    }
}
