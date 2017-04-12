using AbilityUser;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ProjectJedi
{
    public class Projectile_ForceStorm : Projectile_Ability
    {
        private Mesh boltMesh = null;
        private IntVec3 strikeLoc = IntVec3.Invalid;
        private int age = 0;
        private int duration;

        public void ThrowBolt(IntVec3 strikeZone)
        {
            this.strikeLoc = strikeZone;
            SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera();
            if (!strikeLoc.IsValid)
            {
                strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(Caster.Map) && !Caster.Map.roofGrid.Roofed(sq), Caster.Map, 1000);
            }
            this.boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            GenExplosion.DoExplosion(strikeLoc, Caster.Map, 1.9f, DamageDefOf.Flame, null, null, null, null, null, 0f, 1, false, null, 0f, 1);
            Vector3 loc = strikeLoc.ToVector3Shifted();
            for (int i = 0; i < 4; i++) 
            {
                MoteMaker.ThrowSmoke(loc, Caster.Map, 1.5f);
                MoteMaker.ThrowMicroSparks(loc, Caster.Map);
                MoteMaker.ThrowLightningGlow(loc, Caster.Map, 1.5f);
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(strikeLoc, Caster.Map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

        public override void PostImpactEffects(Thing hitThing)
        {
            if (hitThing != null)
            {

                Pawn victim = hitThing as Pawn;
                if (victim != null)
                {
                    this.duration = Rand.Range(30, 60);
                    ThrowBolt(victim.PositionHeld);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            age++;
        }

        protected float LightningBrightness
        {
            get
            {
                if (this.age <= 3)
                {
                    return (float)this.age / 3f;
                }
                return 1f - (float)this.age / (float)this.duration;
            }
        }

        public override void Draw()
        {
            if (boltMesh != null)
            {
                Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf((Material)AccessTools.Field(typeof(WeatherEvent_LightningStrike), "LightningMat").GetValue(null), LightningBrightness), 0);
            }
            //base.Comps_PostDraw();
        }

    }
}
