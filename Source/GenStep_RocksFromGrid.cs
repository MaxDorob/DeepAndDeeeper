using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    /// <summary>
    /// The same GenStep but adjustable through the settings
    /// </summary>
    public class GenStep_RocksFromGrid : RimWorld.GenStep_RocksFromGrid
    {
        public override void Generate(Verse.Map map, Verse.GenStepParams parms)
        {
            this.overrideBlotchesPer10kCells = Mod.Settings.mineableModifier;
            base.Generate(map, parms);
        }
    }
}
