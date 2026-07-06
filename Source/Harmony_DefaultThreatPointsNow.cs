using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow))]
    internal static class Harmony_DefaultThreatPointsNow
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var targetMethod = AccessTools.PropertyGetter(typeof(Map), nameof(Map.IsPocketMap));
            var list = instructions.ToList();
            bool patched = false;
            for (var i = 0; i < list.Count; i++)
            {
                if (!patched && list[i].Calls(targetMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.Call(typeof(Harmony_DefaultThreatPointsNow), nameof(ParentIsNull));
                    yield return new CodeInstruction(OpCodes.Brtrue_S, list[i + 1].operand);
                    patched = true;
                }
                yield return list[i];
            }
        }
        public static bool ParentIsNull(Map map)
        {
            return map.PocketMapParent?.sourceMap == null;
        }
    }
}
