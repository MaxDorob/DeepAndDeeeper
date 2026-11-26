using Verse.AI.Group;

namespace Shashlichnik
{
    public class LordToil_MineClusters : LordToil
    {
        public override void UpdateAllDuties()
        {
            foreach (var pawn in this.lord.ownedPawns)
            {
                var mindState = pawn.mindState;
                if (mindState != null)
                {
                    mindState.duty = new Verse.AI.PawnDuty(DefsOf.ShashlichnikMineForever);
                }
            }
        }
    }
}