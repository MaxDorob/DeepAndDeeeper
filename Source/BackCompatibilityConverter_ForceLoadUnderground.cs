using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace Shashlichnik
{
    internal class BackCompatibilityConverter_ForceLoadUnderground : BackCompatibilityConverter
    {
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
            return baseType;
        }

        public override void PostExposeData(object obj)
        {
        }
#pragma warning disable 0618
        public override void PostLoadSavegame(string loadingVersion)
        {
            base.PostLoadSavegame(loadingVersion);
            foreach (var entrance in Find.Maps.SelectMany(x=>x.listerThings.GetThingsOfType<CaveEntrance>()))
            {
                var caveComp = entrance.CaveMapComponent;
                if (caveComp != null && entrance.isCollapsing)
                {
                    caveComp.BeginCollapsing(entrance);
                    caveComp.collapseTick = entrance.collapseTick;
                }
                entrance.isCollapsing = false;
            }
        }
#pragma warning restore 0618
    }
}
