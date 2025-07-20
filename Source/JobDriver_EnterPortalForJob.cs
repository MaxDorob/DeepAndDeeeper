using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace Shashlichnik
{
    public class JobDriver_EnterPortalForJob : JobDriver_EnterPortal
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var map = TargetB.Thing.Map;
            if (base.TryMakePreToilReservations(errorOnFailed) && !map.reservationManager.IsReserved(TargetB))
            {
                ReservationManager.Reservation reservation = new ReservationManager.Reservation(pawn, job, 1, -1, TargetB, null);
                map.reservationManager.reservations.Add(reservation);
#if v16
                map.events.Notify_ReservationAdded(reservation);
#endif
                return true;
            }
            return false;
        }
    }
}
