using RimWorld;
using RimWorld.QuestGen;
using Shashlichnik;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace DeepAndDeeper
{
    [HarmonyLib.HarmonyPatch(typeof(QuestScriptDef), nameof(QuestScriptDef.CanRun), [typeof(Slate), typeof(IIncidentTarget)])]
    internal static class Harmony_CanRunQuest
    {
        public static bool Prefix(IIncidentTarget target, QuestScriptDef __instance, ref bool __result)
        {
            if (target is Map map && map.GetComponent<CaveMapComponent>() != null)
            {
                __result = __instance.GetModExtension<DefModExtension_CanRunInCave>()?.CanRun ?? false;
                return false;
            }
            return true;
        }
    }
}
