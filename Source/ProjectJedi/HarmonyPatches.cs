using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;

namespace ProjectJedi
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.jedi");
            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), "InitializeComps"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("InitializeComps_PostFix")), null);
            harmony.Patch(AccessTools.Method(typeof(Thing), "TakeDamage"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("TakeDamage_PreFix")), null);
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "GetNonMissChance"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("GetNonMissChance_PostFix")));
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "PreApplyDamage"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("PreApplyDamage_PreFix")), null);
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DrawEquipment_PostFix")));
            harmony.Patch(AccessTools.Method(typeof(Pawn), "GetGizmos"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("GetGizmos_PostFix")));
            harmony.Patch(AccessTools.Method(typeof(SkillRecord), "Learn"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Learn_PostFix")));
            harmony.Patch(AccessTools.Method(typeof(StorytellerUtility), "DefaultParmsNow"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DefaultParmsNow_PostFix")));
        }

        // RimWorld.StorytellerUtility
        public static void DefaultParmsNow_PostFix(ref IncidentParms __result, StorytellerDef tellerDef, IncidentCategory incCat, IIncidentTarget target)
        {
            Map map = target as Map;
            if (map != null)
            {
                if (__result.points > 0)
                {
                    try
                    {
                        List<Pawn> forceUsers = map.mapPawns.FreeColonistsSpawned.ToList().FindAll(p => p.GetComp<CompForceUser>() != null);
                        if (forceUsers != null)
                        {
                            foreach (Pawn pawn in forceUsers)
                            {
                                CompForceUser compForce = pawn.GetComp<CompForceUser>();
                                if (compForce.ForceUserLevel > 0)
                                {
                                        __result.points += (20 * compForce.ForceUserLevel);
                                }
                            }
                        }
                    }
                    catch (NullReferenceException)
                    { }
                }
            }
        }

        /// <summary>
        /// Add Force XP every time a pawn learns a skill.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="sDef"></param>
        /// <param name="xp"></param>
        /// <param name="direct"></param>
        public static void Learn_PostFix(SkillRecord __instance, float xp, bool direct = false)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(SkillRecord), "pawn").GetValue(__instance);
            CompForceUser compForce = pawn.TryGetComp<CompForceUser>();
            if (compForce != null)
            {
                if ((__instance.def == SkillDefOf.Melee || __instance.def == SkillDefOf.Shooting))
                {
                    compForce.ForceUserXP += Rand.Range(3, 6);
                }
                else if (Find.TickManager.TicksGame % 70 == 0)
                {
                    compForce.ForceUserXP++;
                }
            }
        }

        public static IEnumerable<Gizmo> gizmoGetter(HediffComp_Shield compHediffShield)
        {
            if (compHediffShield.GetWornGizmos() != null)
            {
                IEnumerator<Gizmo> enumerator = compHediffShield.GetWornGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Gizmo current = enumerator.Current;
                    yield return current;
                }
            }
        }

        public static void GetGizmos_PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            Pawn pawn = __instance;
            if (pawn.health != null)
            {
                if (pawn.health.hediffSet != null)
                {
                    if (pawn.health.hediffSet.hediffs != null && pawn.health.hediffSet.hediffs.Count > 0)
                    {
                        Hediff shieldHediff = pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff x) => x.TryGetComp<HediffComp_Shield>() != null);
                        if (shieldHediff != null)
                        {
                            HediffComp_Shield shield = shieldHediff.TryGetComp<HediffComp_Shield>();
                            if (shield != null)
                            {
                                __result = __result.Concat<Gizmo>(gizmoGetter(shield));
                            }
                        }
                    }
                }
            }
        }

        // Verse.PawnRenderer
        public static void DrawEquipment_PostFix(PawnRenderer __instance, Vector3 rootLoc)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);
                if (pawn.health != null)
                {
                    if (pawn.health.hediffSet != null)
                    {
                        if (pawn.health.hediffSet.hediffs != null && pawn.health.hediffSet.hediffs.Count > 0)
                        {
                            Hediff shieldHediff = pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff x) => x.TryGetComp<HediffComp_Shield>() != null);
                            if (shieldHediff != null)
                            {
                                HediffComp_Shield shield = shieldHediff.TryGetComp<HediffComp_Shield>();
                                if (shield != null)
                                {
                                    shield.DrawWornExtras();
                                }
                            }
                        }
                    }
                }
            
        }


        // Verse.Pawn_HealthTracker
        public static bool PreApplyDamage_PreFix(Pawn_HealthTracker __instance, DamageInfo dinfo, out bool absorbed)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
            if (pawn != null)
            {

                if (pawn.health.hediffSet.hediffs != null && pawn.health.hediffSet.hediffs.Count > 0)
                {
                    Hediff shieldHediff = pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff x) => x.TryGetComp<HediffComp_Shield>() != null);
                    if (shieldHediff != null)
                    {
                        HediffComp_Shield shield = shieldHediff.TryGetComp<HediffComp_Shield>();
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

        public static int nonForceUserLightsaberDamage = 10;

        public static void GetNonMissChance_PostFix(Verb_MeleeAttack __instance, LocalTargetInfo target, ref float __result)
        {
            //if (target.Thing != null && target.Thing is Pawn)
            //{
                Pawn attacker = __instance.CasterPawn;
                if (attacker != null)
                {
                    Pawn_EquipmentTracker pawn_EquipmentTracker = attacker.equipment;
                    if (pawn_EquipmentTracker != null)
                    {
                        foreach (ThingWithComps thingWithComps in pawn_EquipmentTracker.AllEquipmentListForReading)
                        {
                            if (thingWithComps != null)
                            {
                                if (thingWithComps.def.IsMeleeWeapon)
                                {
                                    if (thingWithComps.def.defName.Contains("SWSaber_"))
                                    {
                                        CompForceUser compForce = attacker.TryGetComp<CompForceUser>();
                                        if (compForce == null)
                                        {
                                            __result = 0.5f;
                                        }
                                        else if (!compForce.IsForceUser)
                                        {
                                            __result = 0.5f;
                                        }
                                        else
                                        {
                                            float newAccuracy = (float)(__result / 2);

                                            int accuracyPoints = compForce.ForceSkillLevel("PJ_LightsaberAccuracy");
                                            if (accuracyPoints > 0)
                                            {
                                                for (int i = 0; i < accuracyPoints; i++)
                                                {
                                                    newAccuracy += 0.2f;
                                                }
                                            }
                                            __result = newAccuracy;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            //}

            
        }

        public static void TakeDamage_PreFix(Thing __instance, ref DamageInfo dinfo)
        {
            if (dinfo.Instigator != null)
            {
                Pawn attacker = dinfo.Instigator as Pawn;
                if (attacker != null)
                {
                    Pawn_EquipmentTracker pawn_EquipmentTracker = attacker.equipment;
                    if (pawn_EquipmentTracker != null)
                    {
                        foreach (ThingWithComps thingWithComps in pawn_EquipmentTracker.AllEquipmentListForReading)
                        {
                            if (thingWithComps != null)
                            {
                                if (thingWithComps.def.IsMeleeWeapon)
                                {
                                    if (thingWithComps.def.defName.Contains("SWSaber_"))
                                    {
                                        CompForceUser compForce = attacker.TryGetComp<CompForceUser>();
                                        if (compForce == null)
                                        {
                                            dinfo.SetAmount(10);
                                        }
                                        else if (!compForce.IsForceUser)
                                        {
                                            dinfo.SetAmount(10);
                                        }
                                        else
                                        {
                                            int newDamage = (int)(dinfo.Amount / 2);

                                            int offensePoints = compForce.ForceSkillLevel("PJ_LightsaberOffense");
                                            if (offensePoints > 0)
                                            {
                                                for (int i = 0; i < offensePoints; i++)
                                                {
                                                    newDamage += (int)(dinfo.Amount / 5);
                                                }
                                            }
                                            dinfo.SetAmount(newDamage);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void InitializeComps_PostFix(ThingWithComps __instance)
        {
            //Log.Message("1");
            if (__instance != null)
            {
                Pawn p = __instance as Pawn;
                if (p != null)
                {
                    if (p.RaceProps != null && p.RaceProps.Humanlike)
                    {
                        ThingComp thingComp = (ThingComp)Activator.CreateInstance(typeof(CompForceUser));
                        thingComp.parent = __instance;
                        var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(__instance);
                        if (comps != null)
                        {
                            ((List<ThingComp>)comps).Add(thingComp);
                        }
                        thingComp.Initialize(null);
                    }
                }
            }
        }
    }
}
