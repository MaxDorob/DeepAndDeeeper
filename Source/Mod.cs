using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
        static Vector2 buttonSize = new Vector2(150f, 38f);
        static float Margin = 16f;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var y = inRect.yMin;

            Settings.mapSize = (int)Widgets.HorizontalSlider(new Rect(inRect.x, y, inRect.width, buttonSize.y), Settings.mapSize, 100, 400, label: "ShashlichnikMapSize".Translate() + ": " + Settings.mapSize.ToString(), roundTo: 1);
        }
        public override string SettingsCategory()
        {
            return "Deep And Deeper";
        }
    }
}
