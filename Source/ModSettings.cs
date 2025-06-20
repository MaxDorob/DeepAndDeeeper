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

        public int mapSize = 150;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref mapSize, nameof(mapSize), 150);
        }
    }
}
