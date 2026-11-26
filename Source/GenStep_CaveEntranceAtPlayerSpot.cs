using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveEntranceAtPlayerSpot : GenStep
    {
        public override int SeedPart => 82915435;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (MapGenerator.PlayerStartSpotValid)
            {
                GenSpawn.Spawn(ThingMaker.MakeThing(DefsOf.ShashlichnikCaveEntrance, null), MapGenerator.PlayerStartSpot, map, WipeMode.Vanish);
            }
            else
            {
                Log.Error("PlayerSpot is invalid");
            }
        }
    }
}
