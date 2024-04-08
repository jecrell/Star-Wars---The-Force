using AbilityUser;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ProjectJedi
{
    public class Projectile_ForceStorm : Projectile_Ability
    {
        private int age;
        private Mesh boltMesh;
        private int duration;
        private IntVec3 strikeLoc = IntVec3.Invalid;
        private bool thrown;

        protected float LightningBrightness
        {
            get
            {
                if (age <= 3)
                {
                    return age / 3f;
                }

                return 1f - (age / (float) duration);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (age < 600)
            {
                return;
            }

            base.Destroy(mode);
        }

        public void ThrowBolt(IntVec3 strikeZone, Pawn victim)
        {
            if (thrown)
            {
                return;
            }

            thrown = true;
            strikeLoc = strikeZone;
            SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera();
            //if (!strikeLoc.IsValid)
            //{
            //    strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(victim.Map) && !victim.Map.roofGrid.Roofed(sq), victim.Map, 1000);
            //}
            boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            GenExplosion.DoExplosion(strikeLoc, victim.Map, 1.9f, DamageDefOf.Flame, null);
            var loc = strikeLoc.ToVector3Shifted();
            for (var i = 0; i < 4; i++)
            {
                FleckMaker.ThrowSmoke(loc, victim.Map, 1.5f);
                FleckMaker.ThrowMicroSparks(loc, victim.Map);
                FleckMaker.ThrowLightningGlow(loc, victim.Map, 1.5f);
            }

            var info = SoundInfo.InMap(new TargetInfo(strikeLoc, victim.Map));
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

        public override void PostImpactEffects(Thing hitThing)
        {
            if (hitThing == null)
            {
                return;
            }

            if (hitThing is not Pawn victim)
            {
                return;
            }

            duration = Rand.Range(30, 60);
            ThrowBolt(victim.Position, victim);
        }

        public override void Tick()
        {
            base.Tick();
            age++;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (boltMesh != null)
            {
                Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather),
                    Quaternion.identity,
                    FadedMaterialPool.FadedVersionOf(
                        (Material) AccessTools.Field(typeof(WeatherEvent_LightningStrike), "LightningMat")
                            .GetValue(null), LightningBrightness), 0);
            }
            //base.Comps_PostDraw();
        }
    }
}