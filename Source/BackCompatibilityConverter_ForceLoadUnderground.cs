using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace Shashlichnik
{
    [StaticConstructorOnStartup]
    internal class BackCompatibilityConverter_ForceLoadUnderground : BackCompatibilityConverter
    {
        static BackCompatibilityConverter_ForceLoadUnderground()
        {
            BackCompatibility.conversionChain.Add(new BackCompatibilityConverter_ForceLoadUnderground());
        }
        public override bool AppliesToVersion(int majorVer, int minorVer)
        {
            return true;
        }

        public override string BackCompatibleDefName(Type defType, string defName, bool forDefInjections = false, XmlNode node = null)
        {
            return defName;
        }

        public override Type GetBackCompatibleType(Type baseType, string providedClassName, XmlNode node)
        {
            return null;
        }

        public override void PostExposeData(object obj)
        {
        }
#pragma warning disable 0618
        public override void PostLoadSavegame(string loadingVersion)
        {
            base.PostLoadSavegame(loadingVersion);
            foreach (var entrance in Find.Maps.SelectMany(x => x.listerThings.GetThingsOfType<CaveEntrance>()).ToList())
            {
                var caveComp = entrance.CaveMapComponent;
                if (caveComp != null && entrance.isCollapsing)
                {
                    caveComp.BeginCollapsing(entrance);
                    caveComp.collapseTick = entrance.collapseTick;
                }
                entrance.isCollapsing = false;
            }
            foreach (var map in Find.Maps.ToList())
            {
                var tracker = map.GetComponent<CaveEntranceTracker>();
                var entrances = map.listerThings.ThingsInGroup(ThingRequestGroup.MapPortal).OfType<CaveEntrance>().ToList();
                foreach (var entrance in entrances)
                {
                    if (!tracker.cavesOpened.ContainsKey(entrance.Position))
                    {
                        tracker.Notify_OpenedAt(entrance.Position);
                    }
                }
            }
        }
#pragma warning restore 0618
    }
}
