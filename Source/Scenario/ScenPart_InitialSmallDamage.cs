using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class ScenPart_InitialSmallDamage : ScenPart
    {
        public FloatRange damage = new FloatRange(3, 7);
        private static IEnumerable<DamageDef> AvailableDamageDefs
        {
            get
            {
                yield return DamageDefOf.Crush;
                yield return DamageDefOf.Blunt;
            }
        }
        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);
            if (Find.GameInitData == null)
            {
                return;
            }

            foreach (var pawn in Find.GameInitData.startingAndOptionalPawns)
            {
                var shockThreshold = pawn.GetStatValue(StatDefOf.PainShockThreshold, true, -1);
                var count = Rand.Range(2, 5);
                for (int i = 0; i < count; i++)
                {
                    var damDef = AvailableDamageDefs.RandomElement();
                    var dInfo = new DamageInfo(damDef, damage.RandomInRange, category: DamageInfo.SourceCategory.Collapse);
                    dInfo.SetBodyRegion(BodyPartHeight.Top, BodyPartDepth.Outside);
                    pawn.TakeDamage(dInfo);
                    var currentPain = pawn.health.hediffSet.PainTotal;
                    if (currentPain > shockThreshold * 0.7f) // don't let pawn instantly collapse
                    {
                        break;
                    }
                }
            }
        }
    }
}
