using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class ScenPart_PlayerPawnsArriveMethod : RimWorld.ScenPart_PlayerPawnsArriveMethod
    {
        public override void GenerateIntoMap(Map map)
        {
            if (Find.GameInitData == null)
            {
                return;
            }
            List<Thing> list = new List<Thing>();
            foreach (ScenPart scenPart in Find.Scenario.AllParts)
            {
                list.AddRange(scenPart.PlayerStartingThings());
            }
            foreach (Pawn key in Find.GameInitData.startingAndOptionalPawns)
            {
                foreach (ThingDefCount t in Find.GameInitData.startingPossessions[key])
                {
                    list.Add(StartingPawnUtility.GenerateStartingPossession(t));
                }
            }

            SpawnThings(map, MapGenerator.PlayerStartSpot, list);
        }
        private void SpawnThings(Map map, IntVec3 cell, List<Thing> startingItems)
        {
            List<List<Thing>> list = new List<List<Thing>>();
            foreach (Pawn item in Find.GameInitData.startingAndOptionalPawns)
            {
                list.Add(new List<Thing>
                {
                    item
                });
            }
            int num = 0;
            foreach (Thing thing in startingItems)
            {
                if (thing.def.CanHaveFaction)
                {
                    thing.SetFactionDirect(Faction.OfPlayer);
                }
                list[num].Add(thing);
                num++;
                if (num >= list.Count)
                {
                    num = 0;
                }
            }

            foreach(Thing thing in list.SelectMany(l => l))
            {
                GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Radius, null, null, null, 2);
            }

        }
    }
}
