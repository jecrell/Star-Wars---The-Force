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
    internal static partial class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("rimworld.jecrell.jedi");

            HarmonyPatches_ForceShield(harmony);
            HarmonyPatches_Lightsaber(harmony);
            HarmonyPatches_Misc(harmony);
        }
    }
}