using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace Shashlichnik.HarmonyPatches
{
    [HarmonyLib.HarmonyPatch(typeof(JobGiver_GetEnergy_Charger), nameof(JobGiver_GetEnergy_Charger.TryGiveJob))]
    [HarmonyPriority(0)] //latest call to let other patches apply
    internal static class Harmony_JobGiver_GetEnergy_Charger
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) //I don't want to call ShouldAutoRecharge again, so transpiler
        {
            var list = instructions.ToList();
            var ldnullToReplace = list.Last(x => x.opcode ==  OpCodes.Ldnull);
            list.InsertRange(list.Count - 2,
                [
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(ldnullToReplace.ExtractLabels()),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    CodeInstruction.Call(typeof(Harmony_JobGiver_GetEnergy_Charger), nameof(GotoToExit))
                ]
                );
            list.Remove(ldnullToReplace); //I suppose that counts as destructive patch, is it?
            return list;
        }

        private static Job GotoToExit(JobGiver_GetEnergy_Charger jobGiver, Pawn pawn)
        {
            if (CaveMapComponent.cachedComponents.ContainsKey(pawn.Map))
            {
                foreach(var exit in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.MapPortal).OfType<CaveExit>())
                {
                    if (exit.exitIfNoJob && pawn.CanReach(exit, PathEndMode.InteractionCell, Danger.Deadly))
                    {
                        return JobMaker.MakeJob(JobDefOf.EnterPortal, exit);
                    }
                }
            }

            return null;
        }
    }
}
