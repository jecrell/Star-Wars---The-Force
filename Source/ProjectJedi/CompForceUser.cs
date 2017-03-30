using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using AbilityUser;
using UnityEngine;

namespace ProjectJedi
{
    /*
     *  Force User Class
     *  
     *  This class initializes a Jedi / Sith with force powers.
     *  Force users use the Force Pool tracker in the needs menu.
     *  When force users use force powers, the pool deteriorates.
     * 
     */
    public class CompForceUser : CompAbilityUser
    {

        private int forceUserLevel = 0;
        public int ForceUserLevel
        {
            get
            {
                return forceUserLevel;
            }
            set
            {
                if (value > forceUserLevel) abilityPoints++;
                forceUserLevel = value;
            }
        }

        private int forceUserXP = 1;
        public int ForceUserXP
        {
            get
            {
                return forceUserXP;
            }
            set
            {
                forceUserXP = value;
            }
        }

        public float XPLastLevel
        {
            get
            {
                float result = 0f;
                if (forceUserLevel > 0) result = forceUserLevel * 600;
                return result;
            }
        }

        public float XPTillNextLevelPercent
        {
            get
            {
                return ((float)(forceUserXP - XPLastLevel) / (float)(ForceUserXPTillNextLevel - XPLastLevel));
            }
        }

        public int ForceUserXPTillNextLevel
        {
            get
            {
                return (forceUserLevel + 1) * 600;
            }
        }

        public List<ForceSkill> forceSkills = new List<ForceSkill>();
        public List<ForceSkill> ForceSkills
        {
            get
            {
                if (forceSkills == null)
                {
                    forceSkills = new List<ForceSkill>
                    {
                        new ForceSkill("lightsaberOffense", 0),
                        new ForceSkill("lightsaberDefense", 0),
                        new ForceSkill("lightsaberAccuracy", 0),
                        new ForceSkill("lightsaberReflection", 0),
                        new ForceSkill("forcePool", 0)
                    };
                }
                return forceSkills;
            }
        }

        public List<ForcePower> forcePowersDark = new List<ForcePower>();
        public List<ForcePower> ForcePowersDark
        {
            get
            {
                if (forcePowersDark == null)
                {
                    forcePowersDark = new List<ForcePower>
                    {
                        new ForcePower("forceRage", TexButton.PJTex_ForceRage, 0),
                        new ForcePower("forceChoke", TexButton.PJTex_ForceChoke, 0),
                        new ForcePower("forceDrain", TexButton.PJTex_ForceDrain, 0),
                        new ForcePower("forceLightning", TexButton.PJTex_ForceLightning, 0),
                        new ForcePower("forceStorm", TexButton.PJTex_ForceStorm, 0)
                    };
                }
                return forcePowersDark;
            }
        }

        public List<ForcePower> forcePowersGray = new List<ForcePower>();
        public List<ForcePower> ForcePowersGray
        {
            get
            {
                if (forcePowersGray == null)
                {
                    forcePowersGray = new List<ForcePower>
                    {
                        new ForcePower("forcePush", TexButton.PJTex_ForcePush, 0),
                        new ForcePower("forcePull", TexButton.PJTex_ForcePull, 0),
                        new ForcePower("forceSpeed", TexButton.PJTex_ForceSpeed, 0)
                    };
                }
                return forcePowersGray;
            }
        }
        public List<ForcePower> forcePowersLight = new List<ForcePower>();
        public List<ForcePower> ForcePowersLight
        {
            get
            {
                if (forcePowersLight == null)
                {
                    forcePowersLight = new List<ForcePower>
                    {
                        new ForcePower("forceHeal", TexButton.PJTex_ForceHeal, 0),
                        new ForcePower("forceHealOther", TexButton.PJTex_ForceHealOther, 0),
                        new ForcePower("forceDefense", TexButton.PJTex_ForceDefense, 0),
                        new ForcePower("mindTrick", TexButton.PJTex_MindTrick, 0),
                        new ForcePower("forceGhost", TexButton.PJTex_ForceGhost, 0)
                    };
                }
                return forcePowersLight;
            }
        }


        public int abilityPoints = 0;

        public int levelLightsaberOff = 4;
        public int levelLightsaberDef = 3;
        public int levelLightsaberAcc = 2;
        public int levelLightsaberRef = 1;
        public int levelForcePool = 0;

        /// <summary>
        /// Keep track of an internal alignment.
        /// As a float value, this allows greater roleplaying possibilities.
        /// </summary>
        private float alignmentValue;
        public float AlignmentValue
        {
            get
            {
                return alignmentValue;
            }
            set
            {
                alignmentValue = value;
            }
        }

        public ForceAlignmentType ForceAlignmentType
        { 
            set
            {
                switch (value)
                {
                    case ForceAlignmentType.Dark:
                        alignmentValue = 0.0f;
                        break;
                    case ForceAlignmentType.Gray:
                    case ForceAlignmentType.None:
                        alignmentValue = 0.5f;
                        break;
                    case ForceAlignmentType.Light:
                        alignmentValue = 1.0f;
                        break;
                }
            }
            get
            {
                if (alignmentValue < 0.4)
                    return ForceAlignmentType.Dark;
                if (alignmentValue < 0.6)
                    return ForceAlignmentType.Gray;
                return ForceAlignmentType.Light;
            }
        }

        /// <summary>
        /// The force pool is where all fatigue and
        /// casting limits are decided.
        /// </summary>
        private Need_ForcePool forcePool;
        public Need_ForcePool ForcePool
        {
            get
            {
                if (forcePool == null) forcePool = abilityUser.needs.TryGetNeed<Need_ForcePool>();
                return forcePool;
            }
        }

        public bool firstTick = false;

        public bool IsForceUser
        {
            get
            {
                if (this.abilityUser != null)
                {
                    if (this.abilityUser.story != null)
                    {
                        if (this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                            this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait) ||
                            this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_ForceSensitive))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public override void CompTick()
        {
            if (Find.TickManager.TicksGame > 200)
            {
                if (Find.TickManager.TicksGame % 30 == 0)
                {
                    if (IsForceUser)
                    {
                        if (!firstTick) PostInitializeTick();
                        base.CompTick();
                        if (forceUserXP > ForceUserXPTillNextLevel) ForceUserLevel += 1;
                        forceUserXP++;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a force user by adding a hidden Hediff that adds their Force Pool needs.
        /// </summary>
        public void PostInitializeTick()
        {
            if (this.abilityUser != null)
            {
                if (this.abilityUser.Spawned)
                {
                    if (this.abilityUser.story != null)
                    {
                        firstTick = true;
                        this.Initialize();
                        ResolveForceTab();
                        ResolveForcePowers();
                        ResolveForcePool();
                    }
                }
            }
        }

        public void ResolveForceTab()
        {
            //PostExposeData();
            //Make the ITab
            IEnumerable<InspectTabBase> tabs = this.abilityUser.GetInspectTabs();
            if (tabs != null && tabs.Count<InspectTabBase>() > 0)
            {
                if (tabs.FirstOrDefault((InspectTabBase x) => x is ITab_Pawn_Force) == null)
                {
                    try
                    {
                        this.abilityUser.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Force)));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(new object[]
                        {
                    "Could not instantiate inspector tab of type ",
                    typeof(ITab_Pawn_Force),
                    ": ",
                    ex
                        }));
                    }
                }
            }
        }

        public void ResolveForcePowers()
        {
            //Set the force alignment
            this.ForceAlignmentType = ForceAlignmentType.Gray; // Default to Gray
            if (this.abilityPowerManager == null) {
                Log.Message("Null handled");
                this.abilityPowerManager = new AbilityPowerManager(this);
            }
            if (this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait))
            {

                this.ForceAlignmentType = ForceAlignmentType.Light;
                // !! DEBUG -- TO BE REMOVED LATER !!
                this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceHealingSelf);
                this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceHealingOther);
            }
            if (this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait))
            {
                this.ForceAlignmentType = ForceAlignmentType.Dark;

                // !! DEBUG -- TO BE REMOVED LATER !!
                this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceDrain);
                this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceLightning);
                this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceStorm);
            }
        }

        public void ResolveForcePool()
        {
            //Add the hediff if no pool exists.
            if (ForcePool == null)
            {
                Hediff forceWielderHediff = abilityUser.health.hediffSet.GetFirstHediffOfDef(ProjectJediDefOf.PJ_ForceWielderHD);
                if (forceWielderHediff != null)
                {
                    forceWielderHediff.Severity = 1.0f;
                }
                else
                {
                    Hediff newForceWielderHediff = HediffMaker.MakeHediff(ProjectJediDefOf.PJ_ForceWielderHD, abilityUser, null);
                    newForceWielderHediff.Severity = 1.0f;
                    abilityUser.health.AddHediff(newForceWielderHediff, null, null);
                }
            }
        }

        /// <summary>
        /// Shows the required alignment (optional), 
        /// alignment change (optional),
        /// and the force pool usage
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        public override string PostAbilityVerbDesc(Verb_UseAbility verb)
        {
            string result = "";
            StringBuilder postDesc = new StringBuilder();
            ForceAbilityDef forceDef = (ForceAbilityDef)verb.useAbilityProps.abilityDef;
            if (forceDef != null)
            {
                string alignDesc = "";
                string changeDesc = "";
                string pointsDesc = "";
                if (forceDef.requiresAlignment)
                {
                    alignDesc = "ForceAbilityDescAlign".Translate(new object[]
                    {
                    forceDef.requiredAlignmentType.ToString(),
                    });
                }
                if (forceDef.changesAlignment)
                {
                    changeDesc = "ForceAbilityDescChange".Translate(new object[]
                    {
                    forceDef.changedAlignmentType.ToString(),
                    forceDef.changedAlignmentRate.ToString("p1")
                    });
                }
                pointsDesc = "ForceAbilityDescPoints".Translate(new object[]
                {
                    forceDef.forcePoolCost.ToString("p1")
                });
                if (alignDesc != "") postDesc.AppendLine(alignDesc);
                if (changeDesc != "") postDesc.AppendLine(changeDesc);
                if (pointsDesc != "") postDesc.AppendLine(pointsDesc);
                result = postDesc.ToString();
            }
            return result;
        }

        /// <summary>
        /// This section checks if the force pool allows for the casting of the spell.
        /// </summary>
        /// <param name="verbAbility"></param>
        /// <param name="reason">Why did we fail?</param>
        /// <returns></returns>
        public override bool CanCastPowerCheck(Verb_UseAbility verbAbility, out string reason)
        {
            reason = "";
            ForceAbilityDef forceDef = (ForceAbilityDef)verbAbility.useAbilityProps.abilityDef;
            if (forceDef != null)
            {
                if (forceDef.requiresAlignment)
                {
                    if (forceDef.requiredAlignmentType != ForceAlignmentType.Gray &&
                    forceDef.requiredAlignmentType != this.ForceAlignmentType)
                    {
                        reason = "WrongAlignment";
                        return false;
                    }
                }
                if (ForcePool != null)
                {
                    if (forceDef.forcePoolCost > 0 &&
                        forceDef.forcePoolCost > ForcePool.CurLevel)
                    {
                        reason = "DrainedForcePool";
                        return false;
                    }
                }

            }
            return true;
        }

        public override List<HediffDef> ignoredHediffs()
        {
            List<HediffDef> newDefs = new List<HediffDef>();
            newDefs.Add(ProjectJediDefOf.PJ_ForceWielderHD);
            return newDefs;
        }



        /// <summary>
        /// This section checks what force abilities were used, and thus their effect on the Jedi's force powers.
        /// </summary>
        public override void PostCastAbilityEffects(Verb_UseAbility verbAbility)
        {
            ForceAbilityDef forceDef = (ForceAbilityDef)verbAbility.useAbilityProps.abilityDef;
            if (forceDef != null)
            {
                if (ForcePool != null)
                {
                    float value = ForcePool.CurLevel - forceDef.forcePoolCost;
                    ForcePool.CurLevel = Mathf.Clamp(value, 0.01f, 0.99f);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.LookValue<float>(ref this.alignmentValue, "alignmentValue", 0.0f);
            Scribe_Values.LookValue<int>(ref this.forceUserLevel, "forceUserLevel", 0);
            Scribe_Values.LookValue<int>(ref this.forceUserXP, "forceUserXP");
            Scribe_Values.LookValue<int>(ref this.levelLightsaberOff, "levelLightsaberOff", 0);
            Scribe_Values.LookValue<int>(ref this.levelLightsaberDef, "levelLightsaberDef", 0);
            Scribe_Values.LookValue<int>(ref this.levelLightsaberAcc, "levelLightsaberAcc", 0);
            Scribe_Values.LookValue<int>(ref this.levelLightsaberRef, "levelLightsaberRef", 0);
            Scribe_Values.LookValue<int>(ref this.levelForcePool, "levelForcePool", 0);
            Scribe_Values.LookValue<int>(ref this.abilityPoints, "abilityPoints", 0);
            Scribe_Collections.LookList<ForcePower>(ref this.forcePowersDark, "forcePowersDark", LookMode.Deep);
            Scribe_Collections.LookList<ForcePower>(ref this.forcePowersGray, "forcePowersGray", LookMode.Deep);
            Scribe_Collections.LookList<ForcePower>(ref this.forcePowersLight, "forcePowersLight", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (forcePowersDark == null)
                {
                    forcePowersDark = new List<ForcePower>
                    {
                        new ForcePower("forceRage", TexButton.PJTex_ForceRage, 0),
                        new ForcePower("forceChoke", TexButton.PJTex_ForceChoke, 0),
                        new ForcePower("forceDrain", TexButton.PJTex_ForceDrain, 0),
                        new ForcePower("forceLightning", TexButton.PJTex_ForceLightning, 0),
                        new ForcePower("forceStorm", TexButton.PJTex_ForceStorm, 0)
                    };
                }
                if (forcePowersGray == null)
                {
                    forcePowersGray = new List<ForcePower>
                    {
                        new ForcePower("forcePush", TexButton.PJTex_ForcePush, 0),
                        new ForcePower("forcePull", TexButton.PJTex_ForcePull, 0),
                        new ForcePower("forceSpeed", TexButton.PJTex_ForceSpeed, 0)
                    };
                }
                if (forcePowersLight == null)
                {
                    forcePowersLight = new List<ForcePower>
                    {
                        new ForcePower("forceHeal", TexButton.PJTex_ForceHeal, 0),
                        new ForcePower("forceHealOther", TexButton.PJTex_ForceHealOther, 0),
                        new ForcePower("forceDefense", TexButton.PJTex_ForceDefense, 0),
                        new ForcePower("mindTrick", TexButton.PJTex_MindTrick, 0),
                        new ForcePower("forceGhost", TexButton.PJTex_ForceGhost, 0)
                    };
                }
            }

            //Log.Message("PostExposeData Called: ForceUser");
        }
    }
}
