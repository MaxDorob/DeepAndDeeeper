using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class ChoiceLetter_AcceptLostGroup : ChoiceLetter
    {
        public override bool CanDismissWithRightClick
        {
            get
            {
                return false;
            }
        }

        public Map MapToUse
        {
            get
            {
                Map result;
                if ((result = this.overrideMap) == null)
                {
                    result = (this.lookTargets.PrimaryTarget.Map ?? Find.AnyPlayerHomeMap);
                }
                return result;
            }
        }

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (base.ArchivedOnly)
                {
                    yield return base.Option_Close;
                }
                else
                {
                    DiaOption diaOption = new DiaOption("AcceptButton".Translate());
                    DiaOption optionReject = new DiaOption("RejectLetter".Translate());
                    diaOption.action = delegate ()
                    {
                        Find.SignalManager.SendSignal(new Signal(this.signalAccept, false));
                        Find.LetterStack.RemoveLetter(this);
                    };
                    diaOption.resolveTree = true;
                    optionReject.action = delegate ()
                    {
                        Find.SignalManager.SendSignal(new Signal(this.signalReject, false));
                        Find.LetterStack.RemoveLetter(this);
                    };
                    optionReject.resolveTree = true;
                    Map mapToUse = this.MapToUse;
                    if (mapToUse == null)
                    {
                        diaOption.Disable("CannotAcceptQuestNoMap".Translate());
                    }
                    else if (mapToUse.Tile.LayerDef.isSpace)
                    {
                        PlanetLayerDef layerDef = mapToUse.Tile.LayerDef;
                        diaOption.Disable("CannotAcceptQuestFromLayer".Translate(layerDef.gerundLabel.Named("GERUND"), layerDef.Named("LAYER")));
                    }
                    yield return diaOption;
                    yield return optionReject;
                    if (this.lookTargets.IsValid())
                    {
                        yield return base.Option_JumpToLocationAndPostpone;
                    }
                    yield return base.Option_Postpone;
                    optionReject = null;
                }
                yield break;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref this.signalAccept, "signalAccept", null, false);
            Scribe_Values.Look<string>(ref this.signalReject, "signalReject", null, false);
            Scribe_References.Look<Map>(ref this.overrideMap, "overrideMap", false);
        }

        public string signalAccept;

        public string signalReject;

        public Map overrideMap;
    }
}
