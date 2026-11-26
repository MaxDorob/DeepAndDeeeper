using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.CanFireNow))]
    internal static class Harmony_IncidentWorker_TryExecute_Cave
    {
        public static bool Prefix(IncidentParms parms, IncidentWorker __instance, ref bool __result)
        {
            var canRunExt = __instance.def.GetModExtension<DefModExtension_CanRunInCave>();
            if (canRunExt != null && !canRunExt.canRun)
            {
                if (parms.target is Map map)
                {
                    if (CaveMapComponent.cachedComponents.TryGetValue(map, out _))
                    {
                        __result = false;
                        return false;
                    }
                }
                else if (Find.Maps.All(m => m.GetComponent<CaveMapComponent>() != null))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
