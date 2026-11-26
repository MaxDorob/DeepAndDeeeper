using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace Shashlichnik
{
    public class GenStep_LostHostileGroup : GenStep_LostGroup
    {
        protected override LordJob CreateLordJob() => new LordJob_DefendPoint();
        protected override Faction GetRandomGroupFaction() => Find.FactionManager.RandomEnemyFaction(true, true, false);
    }
}
