using RimWorld;
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
            if (ModsConfig.BiotechActive)
            {
                sitesChances.Add(SitePartDefOf.AncientComplex_Mechanitor, 0.1f);
            }
        }
        Site site;
        static Dictionary<SitePartDef, float> sitesChances = new Dictionary<SitePartDef, float>()
        {
            { SitePartDefOf.AncientComplex, 0.5f }
        };
        public override Map GetOtherMap()
        {
            Map map = null;
            if (site == null)
            {
                int limit = 30;
                int tile = -1;
                while (limit > 0 && !TileFinder.TryFindNewSiteTile(out tile, 2, 6, nearThisTile: OriginalMap.Tile))
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
                var caveEntrance = map.listerThings.ThingsOfDef(DefsOf.CaveEntrance).FirstOrDefault() as CaveEntrance;
                if (caveEntrance != null)
                {
                    caveEntrance.caveExit = this;
                    caveEntrance.cave = this.Map;
                    caveEntrance.TicksToOpen = 0;
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
