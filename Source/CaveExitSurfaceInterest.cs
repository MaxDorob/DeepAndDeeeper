﻿using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class CaveExitSurfaceInterest : CaveExit
    {
        static CaveExitSurfaceInterest()
        {
            sitesChances = new Dictionary<SitePartDef, float>();

            if (ModsConfig.IdeologyActive)
            {
                sitesChances.Add(SitePartDefOf.AncientComplex, 0.5f);
            }

            if (ModsConfig.BiotechActive)
            {
                sitesChances.Add(SitePartDefOf.AncientComplex_Mechanitor, 0.1f);
            }
        }
        Site site;
        public static Dictionary<SitePartDef, float> sitesChances;
        public override Map GetOtherMap()
        {
            Map map = null;
            if (site == null)
            {
                int limit = 30;
#if v16
                PlanetTile tile = default;
#else
                int tile = -1;
#endif
                while (limit > 0 && !TileFinder.TryFindNewSiteTile(out tile, 2, 6))
                {
                    limit--;
                    if (limit <= 0)
                    {
                        Log.Error("Can't find a tile for a new site");
                        return OriginalMap;
                    }
                }

                site = SiteMaker.MakeSite([sitesChances.RandomElementByWeight(x => x.Value).Key, DefsOf.ShashlichnikCaveEnter], tile, null);
                Find.WorldObjects.Add(site);
                map = GetOrGenerateMapUtility.GetOrGenerateMap(site.Tile, null);
                var caveEntrance = map.listerThings.ThingsOfDef(DefsOf.ShashlichnikCaveEntrance).FirstOrDefault() as CaveEntrance;
                if (caveEntrance != null)
                {
                    caveEntrance.caveExit = this;
                    caveEntrance.cave = this.Map;
                    caveEntrance.TicksToOpen = 0;
                    caveEntrance.SetFaction(RimWorld.Faction.OfPlayer);
                    this.caveEntrance = caveEntrance;
                }
                else
                {
                    Log.Error("Can't find a cave entrance after map generation");
                }
            }
            return map ?? GetOrGenerateMapUtility.GetOrGenerateMap(site.Tile, null);
        }
        public override IntVec3 GetDestinationLocation()
        {

            return caveEntrance?.Position ?? new IntVec3(3, 0, 3);
        }
        protected Map OriginalMap => this.Map.GetComponent<CaveMapComponent>().caveEntrance.Map;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref site, nameof(site));
        }
    }
}
