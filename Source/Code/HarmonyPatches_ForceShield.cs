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

        public static void HarmonyPatches_ForceShield(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.GetGizmos)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetGizmos_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(PawnRenderUtility), nameof(PawnRenderUtility.DrawEquipmentAndApparelExtras)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DrawEquipmentAndApparelExtras_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(PreApplyDamage_PreFix)));
        }

        //Force Shield Gizmos Getter
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
        
        //Force Shield Gizmos Patch 2
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

        // Draw the Force Shield
        // Verse.PawnRenderer
        public static void DrawEquipmentAndApparelExtras_PostFix
            (Pawn pawn, Vector3 drawPos, Rot4 facing, PawnRenderFlags flags)
        {
            if (pawn?.health?.hediffSet?.hediffs == null || pawn.health.hediffSet.hediffs.Count <= 0)
            {
                return;
            }

            var shieldHediff =
                pawn.health.hediffSet.hediffs.FirstOrDefault(x =>
                    x.TryGetComp<HediffComp_Shield>() != null);
            var shield = shieldHediff?.TryGetComp<HediffComp_Shield>();
            shield?.DrawWornExtras();
        }


        // Use the Force Shield to prevent damage
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
    }
}