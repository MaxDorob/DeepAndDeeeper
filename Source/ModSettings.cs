using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    internal class ModSettings : Verse.ModSettings
    {

        public ModSettings() : base()
        {
        }
        public bool AnomalyEffectsEnabled => ModsConfig.AnomalyActive && enableAnomalyEffects;


        public bool enableAnomalyEffects = true;
        public int mapSize = 150;
        public float undergroundTemperatureModifier = 0.6f;
        public bool stabilitySystemEnabled = true;
        public bool landslidesEnabled = true;
        public float mineableModifier = 7;

        public float nestedCaveEntranceChance = 0.15f;
        public bool enableNestedJobSearch = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref mapSize, nameof(mapSize), 150);
            Scribe_Values.Look(ref undergroundTemperatureModifier, nameof(undergroundTemperatureModifier), 0.6f);
            Scribe_Values.Look(ref enableAnomalyEffects, nameof(enableAnomalyEffects), true);
            Scribe_Values.Look(ref landslidesEnabled, nameof(landslidesEnabled), true);
            Scribe_Values.Look(ref stabilitySystemEnabled, nameof(stabilitySystemEnabled), true);
            Scribe_Values.Look(ref mineableModifier, nameof(mineableModifier), 7f);
            Scribe_Values.Look(ref nestedCaveEntranceChance, nameof(nestedCaveEntranceChance), 0.15f);
            Scribe_Values.Look(ref enableNestedJobSearch, nameof(enableNestedJobSearch), true);
        }
    }
}
