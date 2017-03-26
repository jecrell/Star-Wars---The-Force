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
    //Placeholder!
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.starwars.force");
            //harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "AddEquipment"), null, new HarmonyMethod(typeof(HarmonyPatches).GetMethod("AddEquipment_PostFix")), null);
        }
    }
}
