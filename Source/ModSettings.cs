using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    internal class ModSettings : Verse.ModSettings
    {

        public ModSettings() : base()
        {
        }
        public bool AnomalyEffectsEnabled => ModsConfig.AnomalyActive;
        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
