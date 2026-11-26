using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class WorkGiver_GoDownIfJobUnderground_Miner : WorkGiver_GoDownIfJobUnderground
    {
        List<Designation> tmpDesignations = new List<Designation>();
        protected override IEnumerable<Thing> PotentialTargetsUnderground(CaveExit caveExit)
        {
            var map = caveExit.Map;
            tmpDesignations.Clear();
            tmpDesignations.AddRange(map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Mine));
            tmpDesignations.AddRange(map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.MineVein));
            foreach (Designation designation in tmpDesignations)
            {
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 c = designation.target.Cell + GenAdj.AdjacentCells[i];
                    if (c.InBounds(map) && c.Walkable(map))
                    {
                        Mineable firstMineable = designation.target.Cell.GetFirstMineable(map);
                        if (firstMineable != null)
                        {
                            yield return firstMineable;
                        }
                        break;
                    }
                }
            }
        }
    }
}
