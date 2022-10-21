using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForcePush : DamageWorker_ForceLeveled
    {
        public Vector3 PushResult(Thing thingToPush, int pushDist, out bool collision)
        {
            var origin = thingToPush.TrueCenter();
            var result = origin;
            var collisionResult = false;
            for (var i = 1; i <= pushDist; i++)
            {
                var pushDistX = i;
                var pushDistZ = i;
                if (origin.x < Caster.TrueCenter().x)
                {
                    pushDistX = -pushDistX;
                }

                if (origin.z < Caster.TrueCenter().z)
                {
                    pushDistZ = -pushDistZ;
                }

                var tempNewLoc = new Vector3(origin.x + pushDistX, 0f, origin.z + pushDistZ);
                if (tempNewLoc.ToIntVec3().Standable(Caster.Map))
                {
                    result = tempNewLoc;
                }
                else
                {
                    if (thingToPush is not Pawn)
                    {
                        continue;
                    }

                    //target.TakeDamage(new DamageInfo(DamageDefOf.Blunt, Rand.Range(3, 6), -1, null, null, null));
                    collisionResult = true;
                    break;
                }
            }

            collision = collisionResult;
            return result;
        }

        public void PushEffect(Thing target, int distance, bool damageOnCollision = false)
        {
            if (target == null || target is not Pawn pawn)
            {
                return;
            }

            var loc = PushResult(target, distance, out var applyDamage);
            if (pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("PJ_ThoughtPush"));
            }

            var flyingObject =
                (FlyingObject) GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
            if (applyDamage && damageOnCollision)
            {
                flyingObject.Launch(Caster, new LocalTargetInfo(loc.ToIntVec3()), target,
                    new DamageInfo(DamageDefOf.Blunt, Rand.Range(8, 10)));
            }
            else
            {
                flyingObject.Launch(Caster, new LocalTargetInfo(loc.ToIntVec3()), target);
            }
        }

        public override void ApprenticeEffect(Thing target)
        {
            PushEffect(target, 8);
        }

        public override void AdeptEffect(Thing target)
        {
            PushEffect(target, 10);
        }

        public override void MasterEffect(Thing target)
        {
            PushEffect(target, 12, true);
        }
    }
}