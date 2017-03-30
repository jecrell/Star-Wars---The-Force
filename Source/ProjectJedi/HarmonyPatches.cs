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
