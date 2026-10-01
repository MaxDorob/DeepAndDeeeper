using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class QuestPart_EnqueueLandslide : QuestPart
    {
        public QuestPart_EnqueueLandslide()
        {

        }
        public QuestPart_EnqueueLandslide(string inSignal, float radius, float chance, int maxCount, IntRange ticksCount, IntVec3 center, Map map)
        {
            this.inSignal = inSignal;
            this.radius = radius;
            this.chance = chance;
            this.maxCount = maxCount;
            this.ticksCount = ticksCount;
            this.center = center;
            this.map = map;
        }

        public string inSignal;
        public float radius = 5.9f;
        public float chance;
        public int maxCount;
        public IntRange ticksCount;
        public IntVec3 center;
        public Map map;
        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (signal.tag != this.inSignal)
            {
                return;
            }
            var count = 0;
            foreach (var cell in GenRadial.RadialCellsAround(center, radius, true))
            {
                if (cell.InBounds(map) && cell.Walkable(map) && !cell.TryGetFirstThing<Pawn>(map, out _) && Rand.Chance(chance))
                {
                    count++;
                    var caveComp = map.GetComponent<CaveMapComponent>();
                    if (caveComp != null)
                    {
                        caveComp.QueueSingleLandslide(cell, ticksCount.RandomInRange);
                    }
                    else
                    {
                        RoofCollapserImmediate.DropRoofInCells(cell, map);
                    }
                    if (count >= maxCount)
                    {
                        break;
                    }
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inSignal, nameof(inSignal));
            Scribe_Values.Look(ref radius, nameof(radius));
            Scribe_Values.Look(ref chance, nameof(chance));
            Scribe_Values.Look(ref maxCount, nameof(maxCount));
            Scribe_Values.Look(ref ticksCount, nameof(ticksCount));
            Scribe_Values.Look(ref center, nameof(center));
            Scribe_References.Look(ref map, nameof(map));
        }
    }
}
