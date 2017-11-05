using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class PawnGhost : Pawn
    {
        public static readonly int ticksToDestroy = 1800; //30 seconds
        private int ticksLeft;
        bool setup = false;


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            ticksLeft = ticksToDestroy;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public void PowersSetup()
        {
            CompForceUser forcePowers = this.GetComp<CompForceUser>();
            if (forcePowers == null)
            {
                ThingComp thingComp = (ThingComp)Activator.CreateInstance(typeof(CompForceUser));
                thingComp.parent = this;
                var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(this);
                if (comps != null)
                {
                    ((List<ThingComp>)comps).Add(thingComp);
                }
                thingComp.Initialize(null);
            }
            forcePowers = this.GetComp<CompForceUser>();
            if (forcePowers != null)
            {
                forcePowers.AlignmentValue = 0.99f;
                for (int o = 0; o < 10; o++)
                {
                    forcePowers.ForceUserLevel += 1;
                    forcePowers.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                    forcePowers.ForceData.AbilityPoints -= 1;
                }
                for (int i = 0; i < 8; i++)
                {
                    forcePowers.ForceUserLevel += 1;
                    forcePowers.LevelUpPower(forcePowers.ForceData.PowersLight.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                    forcePowers.ForceData.AbilityPoints -= 1;
                }
            }
        }
                
        public void FactionSetup()
        {
            Faction ghostFaction = this.Faction;
            if (ghostFaction != null && ghostFaction != Faction.OfPlayerSilentFail)
            {
                foreach (Faction fac in Find.FactionManager.AllFactions)
                {
                    bool hostile = false;
                    if (fac.HostileTo(Faction.OfPlayerSilentFail))
                    {
                        hostile = true;
                    }
                    ghostFaction.SetHostileTo(fac, hostile);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (setup == false && Find.TickManager.TicksGame % 10 == 0)
            {
                setup = true;
                FactionSetup();
                PowersSetup();
            }

            ticksLeft--;
            if (ticksLeft <= 0) this.Destroy();

            if (Spawned)
            {
                if (effecter == null)
                {
                    EffecterDef progressBar = EffecterDefOf.ProgressBar;
                    effecter = progressBar.Spawn();
                }
                else
                {
                    LocalTargetInfo target = this;
                    if (this.Spawned)
                    {
                        effecter.EffectTick(this, TargetInfo.Invalid);
                    }
                    MoteProgressBar mote = ((SubEffecter_ProgressBar)effecter.children[0]).mote;
                    if (mote != null)
                    {
                        float result = 1f - (float)(PawnGhost.ticksToDestroy - this.ticksLeft) / (float)PawnGhost.ticksToDestroy;

                        mote.progress = Mathf.Clamp01(result);
                        mote.offsetZ = -0.5f;
                    }
                }
            }
        }

        Effecter effecter = null;

        public override void DeSpawn()
        {
            if (effecter != null) effecter.Cleanup();
            base.DeSpawn();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0);
        }
    }
}
