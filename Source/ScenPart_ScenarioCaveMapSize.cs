using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class ScenPart_ScenarioCaveMapSize : ScenPart
    {
        private IntVec3 original;
        public override void PreMapGenerate()
        {
            base.PreMapGenerate();
            Find.GameInitData.mapSize = Mod.Settings.mapSize;
        }
        public override void PostGameStart()
        {
            base.PostGameStart();
            Find.World.info.initialMapSize = original;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref original, nameof(original)); //I'm not sure if this is really needed
        }
    }
}
