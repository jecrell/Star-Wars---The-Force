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
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        public static int nonForceUserLightsaberDamage = 10;

        static HarmonyPatches()
        {
            var harmony = new Harmony("rimworld.jecrell.jedi");
            harmony.Patch(AccessTools.Method(typeof(Thing), nameof(Thing.TakeDamage)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(TakeDamage_PreFix)));
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "GetNonMissChance"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetNonMissChance_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(PreApplyDamage_PreFix)));
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DrawEquipment_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.GetGizmos)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetGizmos_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Learn_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultParmsNow)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DefaultParmsNow_PostFix)));
            harmony.Patch(
                AccessTools.Method(typeof(AttackTargetsCache),
                    nameof(AttackTargetsCache.Notify_FactionHostilityChanged)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Notify_FactionHostilityChanged_PostFix)));
        }

        // RimWorld.StorytellerUtility
        public static void DefaultParmsNow_PostFix(ref IncidentParms __result, IncidentCategoryDef incCat,
            IIncidentTarget target)
        {
            if (!(target is Map map))
            {
                return;
            }

            if (!(__result.points > 0))
            {
                return;
            }

            try
            {
                var forceUsers = map.mapPawns.FreeColonistsSpawned.ToList()
                    .FindAll(p => p.GetComp<CompForceUser>() != null);

                foreach (var pawn in forceUsers)
                {
                    var compForce = pawn.GetComp<CompForceUser>();
                    if (compForce.ForceUserLevel > 0)
                    {
                        __result.points += 5 * compForce.ForceUserLevel;
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        /// Add Force XP every time a pawn learns a skill.
        public static void Learn_PostFix(SkillRecord __instance, float xp, bool direct = false)
        {
            var pawn = (Pawn) AccessTools.Field(typeof(SkillRecord), "pawn").GetValue(__instance);
            if (!(xp > 0) || pawn?.TryGetComp<CompForceUser>() is not { } compForce ||
                !(Find.TickManager.TicksGame > compForce.ForceData?.TicksUntilXPGain))
            {
                return;
            }

            var delay = (int) (130 * ModInfo.forceXPDelayFactor);
            if (__instance.def == SkillDefOf.Intellectual || __instance.def == SkillDefOf.Plants)
            {
                delay += (int) (50 * ModInfo.forceXPDelayFactor);
            }

            compForce.ForceData.TicksUntilXPGain = Find.TickManager.TicksGame + delay;
            compForce.ForceUserXP++;
        }

        public static IEnumerable<Gizmo> GizmoGetter(HediffComp_Shield compHediffShield)
        {
            if (compHediffShield.GetWornGizmos() == null)
            {
                yield break;
            }

            using var enumerator = compHediffShield.GetWornGizmos().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return current;
            }
        }

        public static void GetGizmos_PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.health?.hediffSet?.hediffs == null || __instance.health.hediffSet.hediffs.Count <= 0)
            {
                return;
            }

            var shieldHediff =
                __instance.health.hediffSet.hediffs.FirstOrDefault(x =>
                    x.TryGetComp<HediffComp_Shield>() != null);
            var shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
            if (shield != null)
            {
                __result = __result.Concat(GizmoGetter(shield));
            }
        }

        // Verse.PawnRenderer
        public static void DrawEquipment_PostFix(PawnRenderer __instance, Vector3 rootLoc)
        {
            var pawn = (Pawn) AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);
            if (pawn.health?.hediffSet?.hediffs == null || pawn.health.hediffSet.hediffs.Count <= 0)
            {
                return;
            }

            var shieldHediff =
                pawn.health.hediffSet.hediffs.FirstOrDefault(x =>
                    x.TryGetComp<HediffComp_Shield>() != null);
            var shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
            shield?.DrawWornExtras();
        }


        // Verse.Pawn_HealthTracker
        public static bool PreApplyDamage_PreFix(Pawn_HealthTracker __instance, DamageInfo dinfo, out bool absorbed)
        {
            var pawn = (Pawn) AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
            if (pawn != null)
            {
                if (pawn.health.hediffSet.hediffs is {Count: > 0})
                {
                    var shieldHediff =
                        pawn.health.hediffSet.hediffs.FirstOrDefault(x =>
                            x.TryGetComp<HediffComp_Shield>() != null);
                    if (shieldHediff != null)
                    {
                        var shield = shieldHediff.TryGetComp<HediffComp_Shield>();
                        if (shield != null)
                        {
                            if (shield.CheckPreAbsorbDamage(dinfo))
                            {
                                absorbed = true;
                                return false;
                            }
                        }
                    }
                }
            }

            absorbed = false;
            return true;
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

        private static bool IsSWSaber(ThingDef weaponDef)
        {
            return weaponDef is {IsMeleeWeapon: true} && weaponDef.defName.Contains("SWSaber_");
        }

        public static void Notify_FactionHostilityChanged_PostFix(AttackTargetsCache __instance, Faction f1, Faction f2)
        {
            var map = (Map) AccessTools.Field(typeof(AttackTargetsCache), "map").GetValue(__instance);
            var ghost = (PawnGhost) map?.mapPawns.AllPawnsSpawned.FirstOrDefault(x => x is PawnGhost);
            ghost?.FactionSetup();
        }
    }
}