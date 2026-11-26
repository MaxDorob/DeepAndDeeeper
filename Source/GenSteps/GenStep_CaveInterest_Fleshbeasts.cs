using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_Fleshbeasts : GenStep_CaveInterest_Anomaly
    {
        protected override bool TrySpawnInterestAt(Map map, IntVec3 cell)
        {
            int randomInRange = NumFleshbeastsRange.RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Fingerspike, Faction.OfEntities);
                if (CellFinder.TryFindRandomCellNear(cell, map, SleepingFleshbeastSpawnRadius, c => c.Standable(map) && c.GetFirstPawn(map) == null, out var loc) && GenSpawn.Spawn(newThing, loc, map, WipeMode.Vanish).TryGetComp(out CompCanBeDormant compCanBeDormant))
                {
                    compCanBeDormant.ToSleep();
                }
            }
            return true;
        }
        public int SleepingFleshbeastSpawnRadius = 4;
        public IntRange NumFleshbeastsRange = new IntRange(2, 4);

        public override int SeedPart => 2715235;
    }
}
