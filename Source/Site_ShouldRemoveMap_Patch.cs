using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlichnik
{
    [HarmonyLib.HarmonyPatch(typeof(Site), nameof(Site.ShouldRemoveMapNow))]
    internal class Site_ShouldRemoveMap_Patch
    {
        public static void Postfix(Site __instance, ref bool __result)
        {
            if (__result)
            {
                __result = !__instance.Map.listerThings.GetThingsOfType<CaveEntrance>().Any(c => c.Map != null && !c.Map.Disposed);
            }
        }
    }
}
