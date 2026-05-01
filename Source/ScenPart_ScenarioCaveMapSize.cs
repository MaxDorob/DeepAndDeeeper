using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
        public override IEnumerable<Page> GetConfigPages()
        {
            if (Mod.Settings.mapSize < CaveMapSizePage.recommendedMapSize)
            {
                yield return new CaveMapSizePage();
            }
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
        public class CaveMapSizePage : Page
        {
            public const int recommendedMapSize = 200;
            public override string PageTitle => "ShashlichnikDaDCaveSizeMapWarning".Translate();
            public override void DoWindowContents(Rect rect)
            {
                base.DrawPageTitle(rect);
                Rect mainRect = base.GetMainRect(rect, 0f, false);
                Widgets.BeginGroup(mainRect);
                Widgets.Label(rect, "ShashlichnikDaDCaveSizeMapWarningDescription".Translate());
                Widgets.EndGroup();
                base.DoBottomButtons(rect, null, "ShashlichnikDadCaveSizeSetToRecommended".Translate(), new Action(this.SetToRecommended), true, true);
            }

            private void SetToRecommended()
            {
                Mod.Settings.mapSize = recommendedMapSize;
                Mod.Settings.Write();
                DoNext();
            }
        }
    }
}
