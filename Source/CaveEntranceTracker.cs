using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class CaveEntranceTracker : MapComponent
    {
        public CaveEntranceTracker(Map map) : base(map)
        {
        }
        public const float blockRadius = 11.9f;
        internal Dictionary<IntVec3, int> cavesOpened = new Dictionary<IntVec3, int>();
        private bool IsObsolete(int createdAtTick) => Math.Abs(createdAtTick - Find.TickManager.TicksGame) > GenDate.TicksPerDay * 40; // 40 days until you can dig there another cave

        private IEnumerable<IntVec3> cachedBlockedCells;
        private HashSet<IntVec3> constantlyBlockedCells;
        public HashSet<IntVec3> ConstantlyBlockedCells
        {
            get
            {
                return constantlyBlockedCells ??= new HashSet<IntVec3>();
            }
        }
        public IEnumerable<IntVec3> BlockedCells
        {
            get
            {
                if (cachedBlockedCells == null)
                {
                    cachedBlockedCells = cavesOpened.Keys.Where(k => !IsObsolete(cavesOpened[k])).SelectMany(center => GenRadial.RadialCellsAround(center, blockRadius, true)).Union(ConstantlyBlockedCells).ToList();
                }
                return cachedBlockedCells;
            }
        }
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (map.IsHashIntervalTick(GenDate.TicksPerDay * 2))
            {
                RemoveObsolete();
            }
        }
        public void Notify_OpenedAt(IntVec3 position)
        {
            cachedBlockedCells = null;
            cavesOpened[position] = Find.TickManager.TicksGame;
        }

        private void RemoveObsolete()
        {
            var obsoleteCells = cavesOpened.Where(p => IsObsolete(p.Value)).Select(p => p.Key).ToList();
            foreach (var obsoleteCell in obsoleteCells)
            {
                cavesOpened.Remove(obsoleteCell);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                constantlyBlockedCells ??= new HashSet<IntVec3>();
            }
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                RemoveObsolete();
            }
            Scribe_Collections.Look(ref cavesOpened, nameof(cavesOpened));
            Scribe_Collections.Look(ref constantlyBlockedCells, nameof(constantlyBlockedCells));
        }
    }
}
