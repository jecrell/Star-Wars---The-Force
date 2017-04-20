using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForcePull : DamageWorker_ForceLeveled
    {
        public override void ApprenticeEffect(Thing target)
        {
            if (target != null && target.def != null)
            {
                ThingDef tempProjectile = new ThingDef();
                tempProjectile.defName = "PJ_TempProjectile_ForcePull";
                tempProjectile.label = target.Label;
                tempProjectile.graphicData = target.def.graphicData;
                tempProjectile.projectile.damageDef = DamageDefOf.Stun;
                tempProjectile.projectile.damageAmountBase = 0;
                tempProjectile.projectile.speed = 70;
                Projectile projectile = (Projectile)GenSpawn.Spawn(tempProjectile, target.PositionHeld, target.MapHeld);
                projectile.Launch(Caster, target.Position.ToVector3(), Caster.PositionHeld, null);
                target.Position = Caster.Position;
            }
        }
        public override void AdeptEffect(Thing target)
        {
            if (target != null && target.def != null)
            {
                ThingDef tempProjectile = new ThingDef();
                tempProjectile.defName = "PJ_TempProjectile_ForcePull";
                tempProjectile.label = target.Label;
                tempProjectile.graphicData = target.def.graphicData;
                tempProjectile.projectile.damageDef = DamageDefOf.Stun;
                tempProjectile.projectile.damageAmountBase = 0;
                tempProjectile.projectile.speed = 70;
                Projectile projectile = (Projectile)GenSpawn.Spawn(tempProjectile, target.PositionHeld, target.MapHeld);
                projectile.Launch(Caster, target.Position.ToVector3(), Caster.PositionHeld, null);
                target.Position = Caster.Position;
            }
        }
        public override void MasterEffect(Thing target)
        {
            if (target != null && target.def != null)
            {
                ThingDef tempProjectile = new ThingDef();
                tempProjectile.defName = "PJ_TempProjectile_ForcePull";
                tempProjectile.label = target.Label;
                tempProjectile.graphicData = target.def.graphicData;
                tempProjectile.projectile.damageDef = DamageDefOf.Stun;
                tempProjectile.projectile.damageAmountBase = 0;
                tempProjectile.projectile.speed = 70;
                Projectile projectile = (Projectile)GenSpawn.Spawn(tempProjectile, target.PositionHeld, target.MapHeld);
                projectile.Launch(Caster, target.Position.ToVector3(), Caster.PositionHeld, null);
                target.Position = Caster.Position;
            }
        }
    }
}
