using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class ShutUpLetter : StandardLetter
    {
        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                foreach (var item in base.Choices)
                {
                    yield return item;
                }
                yield return Option_ShutUp;
            }
        }
        protected DiaOption Option_ShutUp
        {
            get
            {
                DiaOption diaOption = new DiaOption("ShashlichnikShutUp".Translate());
                diaOption.action = delegate ()
                {
                    foreach (var target in lookTargets.targets)
                    {
                        var spammer = target.Thing?.TryGetComp<SpammerComp>();
                        if (spammer != null)
                        {
                            spammer.messagesLeft = 0;
                        }
                    }
                    Find.LetterStack.RemoveLetter(this);
                };
                diaOption.resolveTree = true;
                return diaOption;
            }
        }
    }
}
