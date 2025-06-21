using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public abstract class GenStep_CaveInterest_Anomaly : GenStep_CaveInterest
    {
        public override void Generate(Map map, GenStepParams parms)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return;
            }
            base.Generate(map, parms);
        }
    }
}
