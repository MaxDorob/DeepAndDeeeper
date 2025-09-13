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
    public class Alert_Cave : Alert
    {
        public Alert_Cave() : base() { }
        public override string GetLabel()
        {
            var caveComp = Find.CurrentMap.GetComponent<CaveMapComponent>();
            if (caveComp != null)
            {
                if (caveComp.StabilityPercent < criticalStability)
                {
                    return "ShashlichnikCaveAboutToCollapse".Translate();
                }
                else
                {
                    return "ShashlichnikCaveUnstable".Translate();
                }
            }
            return base.GetLabel();
        }
        public override TaggedString GetExplanation()
        {
            var sb = new StringBuilder();
            var cave = Find.CurrentMap.GetComponent<CaveMapComponent>();
            if (cave != null)
            {
                sb.AppendLine("ShashlichnikCaveStabilityAdvice".Translate());
                if (Prefs.DevMode)
                {
                    sb.AppendLine($"DEV: stability {cave.StabilityPercent.ToStringPercent()}");
                    sb.AppendLine($"DEV: landslide chance per tick {cave.LandslideChance}");
                    sb.AppendLine($"DEV: mined count{cave.InitialRockCount - cave.CurrentRockCount}");
                    sb.AppendLine($"DEV: {(Find.TickManager.TicksGame - (float)cave.caveEntrance.tickOpened) / (GenDate.TicksPerDay * 12)}");
                    sb.AppendLine($"DEV: buildings {cave.BuildingsImpact.ToStringPercent()}");
                }
            }
            return sb.ToString();
        }
        protected override Color BGColor
        {
            get
            {
                if (Find.CurrentMap.GetComponent<CaveMapComponent>()?.StabilityPercent < criticalStability)
                {
                    float num = Pulser.PulseBrightness(0.5f, Pulser.PulseBrightness(0.5f, 0.6f));
                    return new Color(num, num, num) * Color.red;
                }
                return base.BGColor;

            }
        }

        public override AlertPriority Priority => (Find.CurrentMap.GetComponent<CaveMapComponent>()?.StabilityPercent < criticalStability) ? AlertPriority.Critical : AlertPriority.Medium;
        public override AlertReport GetReport()
        {
            var caveComp = Find.CurrentMap.GetComponent<CaveMapComponent>();
            return caveComp != null && (caveComp.StabilityPercent < dangerousStability || Prefs.DevMode);
        }
        public const float criticalStability = 0.3f;
        public const float dangerousStability = 0.7f;
    }
}
