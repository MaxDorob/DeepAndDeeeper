using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    internal class Mod : Verse.Mod
    {
        public static ModSettings Settings { get; private set; }
        public Mod(ModContentPack content) : base(content)
        {
            Settings = base.GetSettings<ModSettings>();
        }


    }
}
