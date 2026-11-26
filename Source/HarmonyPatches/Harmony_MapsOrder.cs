using RimWorld;
using Shashlichnik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace DeepAndDeeper
{
    [HarmonyLib.HarmonyPatch()]
    internal static class Harmony_MapsOrder
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(ColonistBar).GetNestedTypes(BindingFlags.NonPublic)[0].GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.ReturnType == typeof(int) && m.GetParameters()[0].ParameterType == typeof(Map)); // A stupid but working way to find the method... maybe
            yield break;
        }
        public static void Postfix(Map x, ref int __result)
        {
            if (CaveMapComponent.cachedComponents.TryGetValue(x, out var comp))
            {
                __result = comp.Level;
            }
        }
    }
}
