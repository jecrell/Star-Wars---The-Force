using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace ProjectJedi
{
    public class Projectile_AbilityLaser_ForceLightning : Projectile_AbilityLaser
    {
        public override void Impact_Override(Thing hitThing)
        {
            base.Impact_Override(hitThing);
            if(hitThing == null) return;
            
            //Throw effects along the way
            var path = Map.pathFinder.FindPath(Caster.PositionHeld, hitThing.PositionHeld, 
                TraverseParms.For(
                    mode: TraverseMode.PassAllDestroyableThings, canBashDoors: true,
                    canBashFences: true
                ), 
                Verse.AI.PathEndMode.OnCell);
            if (path == null) return;
            while (path.NodesLeftCount > 0)
            {
                var nodeCurrent = path.ConsumeNextNode();
                if (nodeCurrent == null) break;
                FleckMaker.ThrowSmoke(nodeCurrent.ToVector3(), hitThing.MapHeld, (new FloatRange(0.05f, 0.5f)).RandomInRange);
                if (new FloatRange(0f, 1f).RandomInRange > 0.2f) FleckMaker.ThrowMicroSparks(nodeCurrent.ToVector3(), hitThing.MapHeld);
                FleckMaker.ThrowLightningGlow(nodeCurrent.ToVector3(), hitThing.MapHeld, (new FloatRange(1f, 1.5f)).RandomInRange);
            }

            //Effect at end location
            Vector3 loc = hitThing.Position.ToVector3Shifted();
            for (int i = 0; i < 4; i++)
            {
                FleckMaker.ThrowSmoke(loc, hitThing.MapHeld, 2.5f);
                FleckMaker.ThrowMicroSparks(loc, hitThing.MapHeld);
                FleckMaker.ThrowLightningGlow(loc, hitThing.MapHeld, 2.5f);
            }
        }
    }
}
