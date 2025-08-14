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
            inRect = inRect.ContractedBy(Margin);
            var y = inRect.yMin;
            if (ModsConfig.AnomalyActive)
            {
                Widgets.CheckboxLabeled(new Rect(inRect.x, y, inRect.width, buttonSize.y), "ShashlichnikEnableAnomalyEffects".Translate(), ref Settings.enableAnomalyEffects);
                y += buttonSize.y + Margin;
            }

            Settings.mapSize = (int)Widgets.HorizontalSlider(new Rect(inRect.x, y, inRect.width, buttonSize.y), Settings.mapSize, 100, 400, label: "ShashlichnikMapSize".Translate() + ": " + Settings.mapSize.ToString(), roundTo: 1);
            y += buttonSize.y + Margin;
            var sliderRect = new Rect(inRect.x, y, inRect.width, buttonSize.y);
            Widgets.HorizontalSlider(sliderRect, ref Settings.mineableModifier, new FloatRange(2f, 15f), "ShashlichnikMineableModifier".Translate() + ": " + Settings.mineableModifier.ToStringByStyle(ToStringStyle.FloatOne), roundTo: 0.5f);
            TooltipHandler.TipRegion(sliderRect, new TipSignal("ShashlichnikMineableModifierExplanation".Translate()));
            y += buttonSize.y + Margin;
            Settings.undergroundTemperatureModifier = Widgets.HorizontalSlider(new Rect(inRect.x, y, inRect.width, buttonSize.y), Settings.undergroundTemperatureModifier, 0.2f, 1.5f, label: "ShashlichnikUndergroundTemperatureModifier".Translate() + ": " + Settings.undergroundTemperatureModifier.ToStringByStyle(ToStringStyle.FloatOne), roundTo: 0.1f);
            y += buttonSize.y + Margin;
            Widgets.HorizontalSlider(new Rect(inRect.x, y, inRect.width, buttonSize.y), ref Settings.nestedCaveEntranceChance, new FloatRange(0f, 1f), "ShashlichnikNestedCaveEntranceChance".Translate() + ": " + Settings.nestedCaveEntranceChance.ToStringByStyle(ToStringStyle.FloatTwo), roundTo: 0.01f);
            y += buttonSize.y + Margin;

            Widgets.CheckboxLabeled(new Rect(inRect.x, y, inRect.width, buttonSize.y), "ShashlichnikStabilitySystemEnabled".Translate(), ref Settings.stabilitySystemEnabled);
            y += buttonSize.y;
            Widgets.CheckboxLabeled(new Rect(inRect.x, y, inRect.width, buttonSize.y), "ShashlichnikLandslidesEnabled".Translate(), ref Settings.landslidesEnabled);
            y += buttonSize.y;

            var rect = new Rect(inRect.x, y, inRect.width, buttonSize.y);
            Widgets.CheckboxLabeled(rect, "ShashlichnikNestedJobSearch".Translate(), ref Settings.enableNestedJobSearch);
            TooltipHandler.TipRegion(rect, new TipSignal("ShashlichnikNestedJobSearchExplanation".Translate()));
            y += buttonSize.y;
        }
        public override string SettingsCategory()
        {
            return "Deep And Deeper";
        }
    }
}
