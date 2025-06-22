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
            if (parent.Position.Fogged(parent.Map))
            {
                return;
            }
            base.CompTick();
        }
    }
}
