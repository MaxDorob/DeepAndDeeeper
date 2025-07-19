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
                foreach (var item in ShutUpOptions)
                {
                    yield return item;
                }
            }
        }
        protected IEnumerable<string> ShutUpOptionStrings
        {
            get
            {
                yield return "ShashlichnikShutUp".Translate();
                yield return "ShashlichnikShutUp1".Translate();
                yield return "ShashlichnikShutUp2".Translate();
                yield return "ShashlichnikShutUp3".Translate();
            }
        }
        protected IEnumerable<DiaOption> ShutUpOptions
        {
            get
            {
                foreach (var optionString in ShutUpOptionStrings.TakeRandomDistinct(2))
                {
                    DiaOption diaOption = new DiaOption(optionString);
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
                    yield return diaOption;
                }
            }
        }
    }
}
