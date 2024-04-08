using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    /// <summary>
    ///     A special version of a projectile.
    ///     This one "stores" a base object and "delivers" it.
    /// </summary>
    public class FlyingObject : ThingWithComps
    {
        protected Vector3 destination;
        protected Thing flyingThing;
        public DamageInfo? impactDamage;
        protected Thing launcher;
        protected Vector3 origin;
        protected float speed = 30.0f;
        protected int ticksToImpact;
        protected Thing usedTarget;

        protected int StartingTicksToImpact
        {
            get
            {
                var num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
                if (num < 1)
                {
                    num = 1;
                }

                return num;
            }
        }


        protected IntVec3 DestinationCell => new IntVec3(destination);

        public virtual Vector3 ExactPosition
        {
            get
            {
                var b = (destination - origin) * (1f - (ticksToImpact / (float) StartingTicksToImpact));
                return origin + b + (Vector3.up * def.Altitude);
            }
        }

        public virtual Quaternion ExactRotation => Quaternion.LookRotation(destination - origin);

        public override Vector3 DrawPos => ExactPosition;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Values.Look(ref destination, "destination");
            Scribe_Values.Look(ref ticksToImpact, "ticksToImpact");
            Scribe_References.Look(ref usedTarget, "usedTarget");
            Scribe_References.Look(ref launcher, "launcher");
            Scribe_References.Look(ref flyingThing, "flyingThing");
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing,
            DamageInfo? newDamageInfo = null)
        {
            //Despawn the object to fly
            if (flyingThing.Spawned)
            {
                flyingThing.DeSpawn();
            }

            this.launcher = launcher;
            this.origin = origin;
            impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            if (targ.Thing != null)
            {
                usedTarget = targ.Thing;
            }

            destination = targ.Cell.ToVector3Shifted() +
                          new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
            ticksToImpact = StartingTicksToImpact;
        }

        public override void Tick()
        {
            base.Tick();
            var unused = ExactPosition;
            ticksToImpact--;
            if (!ExactPosition.InBounds(Map))
            {
                ticksToImpact++;
                Position = ExactPosition.ToIntVec3();
                Destroy();
                return;
            }

            Position = ExactPosition.ToIntVec3();
            if (ticksToImpact > 0)
            {
                return;
            }

            if (DestinationCell.InBounds(Map))
            {
                Position = DestinationCell;
            }

            ImpactSomething();
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (flyingThing == null)
            {
                return;
            }

            if (flyingThing is Pawn)
            {
                if (!DrawPos.ToIntVec3().IsValid)
                {
                    return;
                }

                var pawn = flyingThing as Pawn;
                pawn.DrawNowAt(DrawPos);
                //pawn?.Drawer.DrawAt(DrawPos);
                //Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.graphic.MatFront, 0);
            }
            else
            {
                Graphics.DrawMesh(MeshPool.plane10, DrawPos, ExactRotation, flyingThing.def.DrawMatSingle, 0);
            }

            Comps_PostDraw();
        }

        private void ImpactSomething()
        {
            if (usedTarget != null)
            {
                if (usedTarget is Pawn pawn && pawn.GetPosture() != PawnPosture.Standing &&
                    (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f)
                {
                    Impact(null);
                    return;
                }

                Impact(usedTarget);
            }
            else
            {
                Impact(null);
            }
        }

        protected virtual void Impact(Thing hitThing)
        {
            GenSpawn.Spawn(flyingThing, Position, Map);
            if (impactDamage != null)
            {
                for (var i = 0; i < 3; i++)
                {
                    flyingThing.TakeDamage(impactDamage.Value);
                }
            }

            Destroy();
        }
    }
}