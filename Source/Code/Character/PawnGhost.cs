using System;
using System.Collections.Generic;
using System.Linq;
using AbilityUser;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class PawnGhost : PawnSummoned
    {
        public override void PostSummonSetup()
        {
            base.PostSummonSetup();
            if (Spawner?.Faction == Faction.OfPlayerSilentFail)
            {
                FactionSetup();
            }

            PowersSetup();
        }

        public void PowersSetup()
        {
            var forcePowers = GetComp<CompForceUser>();
            if (forcePowers == null)
            {
                var thingComp = (ThingComp) Activator.CreateInstance(typeof(CompForceUser));
                thingComp.parent = this;
                var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(this);
                if (comps != null)
                {
                    ((List<ThingComp>) comps).Add(thingComp);
                }

                thingComp.Initialize(null);
            }

            forcePowers = GetComp<CompForceUser>();
            if (forcePowers == null)
            {
                return;
            }

            forcePowers.AlignmentValue = 0.99f;
            for (var o = 0; o < 10; o++)
            {
                forcePowers.ForceUserLevel += 1;
                forcePowers.ForceData.Skills.InRandomOrder().First(x => x.level < 4).level++;
                forcePowers.ForceData.AbilityPoints -= 1;
            }

            for (var i = 0; i < 8; i++)
            {
                forcePowers.ForceUserLevel += 1;
                forcePowers.LevelUpPower(forcePowers.ForceData.PowersLight.InRandomOrder().First(x => x.level < 2));
                forcePowers.ForceData.AbilityPoints -= 1;
            }
        }

        public void FactionSetup()
        {
            var ghostFaction = Faction;
            if (Faction?.def != FactionDef.Named("PJ_GhostFaction"))
            {
                return;
            }

            if (ghostFaction == null || ghostFaction == Faction.OfPlayerSilentFail)
            {
                return;
            }

            foreach (var fac in Find.FactionManager.AllFactions)
            {
                var unused = fac.HostileTo(Faction.OfPlayerSilentFail);
                ghostFaction.SetRelationDirect(fac, FactionRelationKind.Hostile);
            }
        }
    }
}