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
        public static void HarmonyPatches_Misc(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultParmsNow)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DefaultParmsNow_PostFix)));
            harmony.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Learn_PostFix)));
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
        
        public static void Notify_FactionHostilityChanged_PostFix(AttackTargetsCache __instance, Faction f1, Faction f2)
        {
            var map = (Map) AccessTools.Field(typeof(AttackTargetsCache), "map").GetValue(__instance);
            var ghost = (PawnGhost) map?.mapPawns.AllPawnsSpawned.FirstOrDefault(x => x is PawnGhost);
            ghost?.FactionSetup();
        }
    }
}