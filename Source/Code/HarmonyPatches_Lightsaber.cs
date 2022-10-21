using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ProjectJedi
{
    internal static partial class HarmonyPatches
    {

        public static void HarmonyPatches_Lightsaber(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "GetNonMissChance"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetNonMissChance_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Thing), nameof(Thing.TakeDamage)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(TakeDamage_PreFix)));            
        }

        private static bool IsSWSaber(ThingDef weaponDef)
        {
            return weaponDef is {IsMeleeWeapon: true} && weaponDef.defName.Contains("SWSaber_");
        }
        
        public static void GetNonMissChance_PostFix(Verb_MeleeAttack __instance, ref float __result)
        {
            if (!(__instance.Caster is Pawn attacker))
            {
                return;
            }

            var weapon = __instance.EquipmentSource;
            if (weapon == null || !IsSWSaber(weapon.def))
            {
                return;
            }

            var compForce = attacker.GetComp<CompForceUser>();
            if (compForce == null || !compForce.IsForceUser)
            {
                __result = 0.5f;
            }
            else
            {
                var newAccuracy = __result / 2;

                var accuracyPoints = compForce.ForceSkillLevel("PJ_LightsaberAccuracy");
                if (accuracyPoints > 0)
                {
                    for (var i = 0; i < accuracyPoints; i++)
                    {
                        newAccuracy += 0.2f;
                    }
                }

                __result = newAccuracy;
            }
        }

        public static void TakeDamage_PreFix(ref DamageInfo dinfo)
        {
            if (!(dinfo.Instigator is Pawn attacker))
            {
                return;
            }

            if (!IsSWSaber(dinfo.Weapon))
            {
                return;
            }

            var compForce = attacker.GetComp<CompForceUser>();
            if (compForce == null || !compForce.IsForceUser)
            {
                dinfo.SetAmount(10);
            }
            else
            {
                var newDamage = (int) (dinfo.Amount / 2);

                var offensePoints = compForce.ForceSkillLevel("PJ_LightsaberOffense");
                if (offensePoints > 0)
                {
                    for (var i = 0; i < offensePoints; i++)
                    {
                        newDamage += (int) (dinfo.Amount / 5);
                    }
                }

                dinfo.SetAmount(newDamage);
            }
        }



    }
}