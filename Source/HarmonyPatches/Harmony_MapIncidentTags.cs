using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(MapParent), nameof(MapParent.IncidentTargetTags))]
    internal static class Harmony_MapIncidentTags
    {
        public static IEnumerable<IncidentTargetTagDef> Postfix(IEnumerable<IncidentTargetTagDef> values, MapParent __instance)
        {
            foreach (var value in values)
            {
                yield return value;
            }
            if (__instance.Map?.GetComponent<CaveMapComponent>() != null)
            {
                yield return DefsOf.ShashlichnikMap_Cave;
            }
        }
    }
}
