using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Shashlichnik
{
    [RimWorld.DefOf]
#pragma warning disable 649
    internal static class DefsOf
    {
        public static ThingDef          ShashlichnikCaveExit;
        public static ThingDef          ShashlichnikCaveExitToSurfaceInterest;
        public static MapGeneratorDef   ShashlichnikUnderground;
        public static MapGeneratorDef   ShashlichnikUndergroundLvl2;
        public static MapGeneratorDef   ShashlichnikUndergroundLvl3;
        public static ThingDef          ShashlichnikCaveEntrance;
        public static JobDef            ShashlichnikDigCaveEntrance;
        public static EffecterDef       ShashlichnikImpactDustCloud;
        public static EffecterDef       ShashlichnikCaveCeilingDebris;
        public static EffecterDef       ShashlichnikCaveEntranceCollapseStage1;
        public static EffecterDef       ShashlichnikCaveEntranceCollapseStage2;
        public static EffecterDef       ShashlichnikCaveEntranceCollapsed;
        public static EffecterDef       ShashlichnikBuryCave;
        public static SoundDef          ShashlichnikVanillaCollapsingStage1;
        public static SoundDef          ShashlichnikVanillaCollapsingStage2;
        public static SitePartDef       ShashlichnikCaveEnter;
        public static JobDef            ShashlichnikEnterPortalForJob;
        public static LetterDef         ShashlichnikPositiveEventShutUp;
        public static ThingDef          Beer;
        public static PawnKindDef       ShashlichnikDeepDiver;
        public static DutyDef           ShashlichnikMineForever;
        [MayRequire("det.stoneborn")]
        public static FactionDef        OutlanderRoughStoneborn;
    }
#pragma warning restore 649
}
