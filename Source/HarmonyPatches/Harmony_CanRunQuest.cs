using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Shashlichnik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DeepAndDeeper
{
    [HarmonyLib.HarmonyPatch(typeof(QuestScriptDef), "CanQuestOccurOnTile")]
    internal static class Harmony_CanRunQuest
    {
        public static void Postfix(PlanetTile tile, QuestScriptDef __instance, ref bool __result)
        {
            if (__result && Find.WorldObjects.ObjectsAt(tile).OfType<MapParent>().Where(mp => mp.HasMap).All(mp => CaveMapComponent.cachedComponents.ContainsKey(mp.Map)))
            {
                __result = __instance.GetModExtension<DefModExtension_CanRunInCave>()?.canRun ?? false;
            }
        }
    }
}
