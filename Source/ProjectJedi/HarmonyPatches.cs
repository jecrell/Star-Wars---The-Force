﻿using Harmony;
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
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "GetHitChance"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("GetHitChance_PostFix")));
        }

        public static int nonForceUserLightsaberDamage = 10;

        public static void GetHitChance_PostFix(Verb_MeleeAttack __instance, ref float __result)
        {

                Pawn attacker = __instance.CasterPawn;
                if (attacker != null)
                {
                    ////Log.Message("1");
                    Pawn_EquipmentTracker pawn_EquipmentTracker = attacker.equipment;
                    if (pawn_EquipmentTracker != null)
                    {
                        ////Log.Message("2");
                        foreach (ThingWithComps thingWithComps in pawn_EquipmentTracker.AllEquipment)
                        {
                            ////Log.Message("3");
                            if (thingWithComps != null)
                            {
                                if (thingWithComps.def.IsMeleeWeapon)
                                {
                                    if (thingWithComps.def.defName.Contains("SWSaber_"))
                                    {

                                    Log.Message("Force :: Lightsaber Original Accuracy " + __result.ToString("F"));
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
                                        Log.Message("Force :: Lightsaber Modified Accuracy " + __result.ToString("F"));
                                    }
                                }
                            }
                        }
                    }
                }
            
        }

        public static void TakeDamage_PreFix(Thing __instance, ref DamageInfo dinfo)
        {
            if (dinfo.Instigator != null)
            {
                Pawn attacker = dinfo.Instigator as Pawn;
                if (attacker != null)
                {
                    ////Log.Message("1");
                    Pawn_EquipmentTracker pawn_EquipmentTracker = attacker.equipment;
                    if (pawn_EquipmentTracker != null)
                    {
                        ////Log.Message("2");
                        foreach (ThingWithComps thingWithComps in pawn_EquipmentTracker.AllEquipment)
                        {
                            ////Log.Message("3");
                            if (thingWithComps != null)
                            {
                                if (thingWithComps.def.IsMeleeWeapon)
                                {
                                    if (thingWithComps.def.defName.Contains("SWSaber_"))
                                    {
                                        Log.Message("Force :: Lightsaber Original Damage " + dinfo.Amount.ToString());
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
                                        Log.Message("Force :: Lightsaber Modified Damage " + dinfo.Amount.ToString());
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
                //Log.Message("2");
                Pawn p = __instance as Pawn;
                if (p != null)
                {
                    //Log.Message("3");
                    //if (p.story != null && (p.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                    //                        p.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait)))
                    //{
                        //Log.Message("4");
                        ThingComp thingComp = (ThingComp)Activator.CreateInstance(typeof(CompForceUser));
                        thingComp.parent = __instance;
                        var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(__instance);
                        if (comps != null)
                        {
                            //Log.Message("comps loaded");
                            ((List<ThingComp>)comps).Add(thingComp);
                        }
                        thingComp.Initialize(null);
                    //}
                }
            }
        }
    }
}