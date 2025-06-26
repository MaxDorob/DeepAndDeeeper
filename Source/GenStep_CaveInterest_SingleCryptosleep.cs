using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class GenStep_CaveInterest_SingleCryptosleep : GenStep_CaveInterest
    {
        public GenStep_CaveInterest_SingleCryptosleep() : base()
        {
            this.subCountChances = [new CountChance() { chance = 1f, count = 1 }];
        }

        public override int SeedPart => 8261902;

        protected override bool TrySpawnInterestAt(Map map, IntVec3 thingPos)
        {
            Building_AncientCryptosleepPod cryptosleepPod = (Building_AncientCryptosleepPod)GenSpawn.Spawn(ThingDefOf.AncientCryptosleepPod, thingPos, map, WipeMode.Vanish);
            ThingSetMakerParams parms = default;
            parms.podContentsType = new PodContentsType?(PodContentsType.AncientHostile);
            parms.countRange = new IntRange(1,1);
            List<Thing> generated = ThingSetMakerDefOf.MapGen_AncientPodContents.root.Generate(parms);
            if (generated.Count > 0)
            {
                cryptosleepPod.TryAcceptThing(generated[0]);
            }
            return true;
        }
    }
}
