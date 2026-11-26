using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class ScatterValidator_OtherBuildings : ScattererValidator
    {
        public ScatterValidator_OtherBuildings() : base() { }
        public override bool Allows(IntVec3 c, Map map)
        {
            var building = c.GetEdifice(map);
            if (building == null)
            {
                return true;
            }
            return building.def.destroyable;
        }
    }
}
