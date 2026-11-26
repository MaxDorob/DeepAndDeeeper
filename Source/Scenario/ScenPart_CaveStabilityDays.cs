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
    public class ScenPart_CaveStabilityDays : ScenPart
    {
        public int stabilityDays = 210;
        string buffer;
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            base.DoEditInterface(listing);
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 5f);
            Widgets.TextFieldNumeric(scenPartRect, ref this.stabilityDays, ref buffer);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ stabilityDays.GetHashCode();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref stabilityDays, nameof(stabilityDays), 210);
        }
    }
}
