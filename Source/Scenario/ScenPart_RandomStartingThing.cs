using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Shashlichnik
{
    public class ScenPart_RandomStartingThing : ScenPart_ThingCount
    {
        public List<ThingDef> things = new List<ThingDef>();
        public override string Summary(Scenario scen)
        {
            StringBuilder sb = new StringBuilder("\n");
            sb.AppendInNewLine("ShashlichnikRandomItemSummary".Translate());
            foreach (var thing in things)
            {
                sb.AppendInNewLine($"  - {thing.LabelCap}");
            }
            return sb.ToString();
        }
        public override IEnumerable<ThingDef> PossibleThingDefs()
        {
            return things;
        }
        public override IEnumerable<Thing> PlayerStartingThings()
        {
            yield return GenerateThing(PossibleThingDefs().RandomElement(), 1);
        }
        private Thing GenerateThing(ThingDef def, int num)
        {
            Thing thing = ThingMaker.MakeThing(def, this.stuff);
            CompQuality compQuality;
            if (thing.TryGetComp(out compQuality))
            {
                compQuality.SetQuality(this.quality.GetValueOrDefault(QualityCategory.Normal), new ArtGenerationContext?(ArtGenerationContext.Outsider));
            }
            if (def.Minifiable)
            {
                thing = thing.MakeMinified();
            }
            if (def.IsIngestible && def.ingestible.IsMeal)
            {
                FoodUtility.GenerateGoodIngredients(thing, Faction.OfPlayer.ideos.PrimaryIdeo);
            }
            thing.stackCount = num;
            return thing;
        }
    }
}
