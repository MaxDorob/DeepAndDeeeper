using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class CompEffecter_NonFogged : CompEffecter
    {
        public override void CompTick()
        {
            if (!Mod.Settings.AnomalyEffectsEnabled || parent.Position.Fogged(parent.Map) || (parent is CaveExit caveExit && !caveExit.WayToSurface))
            {
                return;
            }
            base.CompTick();
        }
    }
}
