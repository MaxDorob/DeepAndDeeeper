using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class WorkGiver_GoDownIfJobUnderground_HaulToPortal : WorkGiver_GoDownIfJobUnderground
    {
        protected override IEnumerable<Thing> PotentialTargetsUnderground(CaveExit portal)
        {
            EnterPortalUtility.neededThings.Clear();
            List<TransferableOneWay> leftToLoad = portal.leftToLoad;
            EnterPortalUtility.tmpAlreadyLoading.Clear();
            if (leftToLoad != null)
            {
                List<Pawn> list = portal.Map.mapPawns.PawnsInFaction(Faction.OfPlayer);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].CurJobDef == JobDefOf.HaulToPortal)
                    {
                        JobDriver_HaulToPortal jobDriver_HaulToPortal = (JobDriver_HaulToPortal)list[i].jobs.curDriver;
                        if (jobDriver_HaulToPortal.Container == portal)
                        {
                            TransferableOneWay transferableOneWay = TransferableUtility.TransferableMatchingDesperate(jobDriver_HaulToPortal.ThingToCarry, leftToLoad, TransferAsOneMode.PodsOrCaravanPacking);
                            if (transferableOneWay != null)
                            {
                                int num = 0;
                                if (EnterPortalUtility.tmpAlreadyLoading.TryGetValue(transferableOneWay, out num))
                                {
                                    EnterPortalUtility.tmpAlreadyLoading[transferableOneWay] = num + jobDriver_HaulToPortal.initialCount;
                                }
                                else
                                {
                                    EnterPortalUtility.tmpAlreadyLoading.Add(transferableOneWay, jobDriver_HaulToPortal.initialCount);
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < leftToLoad.Count; j++)
                {
                    TransferableOneWay transferableOneWay2 = leftToLoad[j];
                    int num2;
                    if (!EnterPortalUtility.tmpAlreadyLoading.TryGetValue(leftToLoad[j], out num2))
                    {
                        num2 = 0;
                    }
                    if (transferableOneWay2.CountToTransfer - num2 > 0)
                    {
                        for (int k = 0; k < transferableOneWay2.things.Count; k++)
                        {
                            EnterPortalUtility.neededThings.Add(transferableOneWay2.things[k]);
                        }
                    }
                }
            }
            return EnterPortalUtility.neededThings;
        }
    }
}
