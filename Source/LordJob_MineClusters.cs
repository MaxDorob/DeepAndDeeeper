using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace Shashlichnik
{
    public class LordJob_MineClusters : LordJob
    {
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            stateGraph.AddToil(new LordToil_MineClusters());
            return stateGraph;
        }
    }
}
