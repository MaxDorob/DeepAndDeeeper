using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class CaveParams : DefModExtension //Not sure if this must be GenStep or DefModExt
    {
        public int? daysToCollapse;
        public MapGeneratorDef caveExitMapGenerator;
    }
}
