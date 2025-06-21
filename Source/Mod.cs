using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    [StaticConstructorOnStartup]
    internal class Mod : Verse.Mod
    {
        static HarmonyLib.Harmony harmony;
        static Mod()
        {
            harmony = new HarmonyLib.Harmony("DeepAndDeeper");
            harmony.PatchAll();
        }
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
            y += buttonSize.y + Margin;
            Settings.undergroundTemperatureModifier = Widgets.HorizontalSlider(new Rect(inRect.x, y, inRect.width, buttonSize.y), Settings.undergroundTemperatureModifier, 0.2f, 1.5f, label: "ShashlichnikUndergroundTemperatureModifier".Translate() + ": " + Settings.undergroundTemperatureModifier.ToStringByStyle(ToStringStyle.FloatOne), roundTo: 0.1f);
        }
        public override string SettingsCategory()
        {
            return "Deep And Deeper";
        }
    }
}
