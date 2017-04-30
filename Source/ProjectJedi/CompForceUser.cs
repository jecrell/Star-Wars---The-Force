using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using AbilityUser;
using UnityEngine;
using Verse.Sound;

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
        #region Variables
        private int forceUserLevel = 0;
        private int forceUserXP = 1;
        public List<ForceSkill> forceSkills;
        public List<ForcePower> forcePowersLight;
        public List<ForcePower> forcePowersGray;
        public List<ForcePower> forcePowersDark;
        public bool forcePowersInitialized = false;

        public int abilityPoints = 0;
        public int canMeditateTicks = 0;
        private float alignmentValue;
        public bool firstTick = false;

        public Faction affiliation = null;
        public int affiliationTicks = 0;

        #endregion Variables

        #region PowerLists
        public List<ForceSkill> ForceSkills
        {
            get
            {
                if (forceSkills == null)
                {
                    forceSkills = new List<ForceSkill>
                    {
                        new ForceSkill("PJ_LightsaberOffense", "PJ_LightsaberOffense_Desc"),
                        new ForceSkill("PJ_LightsaberDefense", "PJ_LightsaberDefense_Desc"),
                        new ForceSkill("PJ_LightsaberAccuracy", "PJ_LightsaberAccuracy_Desc"),
                        new ForceSkill("PJ_LightsaberReflection", "PJ_LightsaberReflection_Desc"),
                        new ForceSkill("PJ_ForcePool", "PJ_ForcePool_Desc")
                    };
                }
                return forceSkills;
            }
        }
        public List<ForcePower> ForcePowersDark
        {
            get
            {
                if (forcePowersDark == null)
                {
                    forcePowersDark = new List<ForcePower>
                    {
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceRage_Apprentice, ProjectJediDefOf.PJ_ForceRage_Adept, ProjectJediDefOf.PJ_ForceRage_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceChoke_Apprentice, ProjectJediDefOf.PJ_ForceChoke_Adept, ProjectJediDefOf.PJ_ForceChoke_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceDrain_Apprentice, ProjectJediDefOf.PJ_ForceDrain_Adept, ProjectJediDefOf.PJ_ForceDrain_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceLightning_Apprentice, ProjectJediDefOf.PJ_ForceLightning_Adept, ProjectJediDefOf.PJ_ForceLightning_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceStorm_Apprentice, ProjectJediDefOf.PJ_ForceStorm_Adept, ProjectJediDefOf.PJ_ForceStorm_Master })
                    };
                }
                return forcePowersDark;
            }
        }
        public List<ForcePower> ForcePowersGray
        {
            get
            {
                if (forcePowersGray == null)
                {
                    forcePowersGray = new List<ForcePower>
                    {
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForcePush_Apprentice, ProjectJediDefOf.PJ_ForcePush_Adept, ProjectJediDefOf.PJ_ForcePush_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForcePull_Apprentice, ProjectJediDefOf.PJ_ForcePull_Adept, ProjectJediDefOf.PJ_ForcePull_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceSpeed_Apprentice, ProjectJediDefOf.PJ_ForceSpeed_Adept, ProjectJediDefOf.PJ_ForceSpeed_Master }),
                    };
                }
                return forcePowersGray;
            }
        }
        public List<ForcePower> ForcePowersLight
        {
            get
            {
                if (forcePowersLight == null)
                {
                    forcePowersLight = new List<ForcePower>
                    {
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceHealingSelf_Apprentice, ProjectJediDefOf.PJ_ForceHealingSelf_Adept, ProjectJediDefOf.PJ_ForceHealingSelf_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceHealingOther_Apprentice, ProjectJediDefOf.PJ_ForceHealingOther_Adept, ProjectJediDefOf.PJ_ForceHealingOther_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceDefense_Apprentice, ProjectJediDefOf.PJ_ForceDefense_Adept, ProjectJediDefOf.PJ_ForceDefense_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_MindTrick_Apprentice, ProjectJediDefOf.PJ_MindTrick_Adept, ProjectJediDefOf.PJ_MindTrick_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceGhost_Apprentice, ProjectJediDefOf.PJ_ForceGhost_Adept, ProjectJediDefOf.PJ_ForceGhost_Master })
                    };
                }
                return forcePowersLight;
            }
        }
        #endregion PowerLists

        #region Levels
        public int ForceUserLevel
        {
            get
            {
                return forceUserLevel;
            }
            set
            {
                if (value > forceUserLevel)
                {
                    abilityPoints++;
                    if (forceUserXP < value * 600)
                    {
                        forceUserXP = value * 600;
                    }
                }
                forceUserLevel = value;
            }
        }
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
                if (ForceUserLevel > 0) result = ForceUserLevel * 600;
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
        public int ForceSkillLevel(string skillName)
        {
            int result = 0;
            ForceSkill skillCheck = ForceSkills.FirstOrDefault((ForceSkill x) => x.label == skillName);
            if (skillCheck != null)
            {
                result = skillCheck.level;
            }
            return result;
        }
        public int DarksidePoints
        {
            get
            {
                int result = 0;
                if (ForcePowersDark != null && ForcePowersDark.Count > 0)
                {
                    foreach (ForcePower power in ForcePowersDark)
                    {
                        result += power.level;
                    }
                }
                return result;
            }
        }
        public int LightsidePoints
        {
            get
            {
                int result = 0;
                if (ForcePowersLight != null && ForcePowersLight.Count > 0)
                {
                    foreach (ForcePower power in ForcePowersLight)
                    {
                        result += power.level;
                    }
                }
                return result;
            }
        }

        public void LevelUp(bool hideNotification = false)
        {
            ForceUserLevel += 1;
            if (ForceUserLevel == 1)
            {

                if (!hideNotification)
                {
                    Messages.Message("PJ_ForcePowersUnlocked".Translate(new object[]
                    {
                                    this.parent.Label
                    }), MessageSound.Silent);
                    Find.LetterStack.ReceiveLetter("PJ_ForceAwakensLabel".Translate(), "PJ_ForceAwakensDesc".Translate(new object[]
                    {
                        this.parent.Label
                        }), LetterType.Good, this.parent, null);
                }
                SoundDef.Named("PJ_ForcePowersUnlocked").PlayOneShotOnCamera();
            }
            else
            {
                if (!hideNotification) Messages.Message("PJ_LevelUp".Translate(new object[]
                {
                this.parent.Label
                }), MessageSound.Benefit);
                UpdateAlignment();
            }

        }
        public void LevelUpPower(ForcePower power)
        {
            this.RemovePawnAbility(power.abilityDef);
            power.level++;
            this.AddPawnAbility(power.abilityDef);
        }
        /// <summary>
        /// Updates the alignment after a level up or force power casting
        /// </summary>
        public void UpdateAlignment()
        {
            //Change traits...
            Trait jediTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            Trait sithTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            Trait grayTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            Trait sensitiveTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);

            //Clear traits.
            if (jediTrait != null) LoseTrait(this.abilityUser.story.traits, jediTrait);
            if (sithTrait != null) LoseTrait(this.abilityUser.story.traits, sithTrait);
            if (grayTrait != null) LoseTrait(this.abilityUser.story.traits, grayTrait);
            if (sensitiveTrait != null) LoseTrait(this.abilityUser.story.traits, sensitiveTrait);

            //Jedi
            int degree = 0;

            if (ForceUserLevel > 14)
            {
                degree = 4;
            }
            else if (ForceUserLevel > 8)
            {
                degree = 3;
            }
            else if (ForceUserLevel > 3)
            {
                degree = 2;
            }
            else if (ForceUserLevel > 0)
            {
                degree = 1;
            }

            if (AlignmentValue > 0.75)
            {
                this.abilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_JediTrait, degree, true));
                return;
            }
            //Gray
            else if (AlignmentValue >= 0.25 && AlignmentValue <= 0.75)
            {
                this.abilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_GrayTrait, degree, true));
                return;
            }
            //Sith
            else
            {
                this.abilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_SithTrait, degree, true));
                return;
            }

        }
        #endregion Levels

        #region Alignment
        /// <summary>
        /// Keep track of an internal alignment.
        /// As a float value, this allows greater roleplaying possibilities.
        /// </summary>
        public float AlignmentValue
        {
            get
            {
                return alignmentValue;
            }
            set
            {     
                alignmentValue = Mathf.Clamp(value, 0.0f, 1.0f);
                if (ForceUserLevel > 0) UpdateAlignment();
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
        #endregion Alignment

        #region Affiliation
        //public void SetAffiliation(Faction newFaction)
        //{
        //    affiliation = newFaction;
        //    affiliationTicks = Find.TickManager.TicksGame + GenDate.TicksPerSeason + Rand.Range(-120000, 120000);
        //}

        //public void BreakAffiliation(Faction newFaction)
        //{
        //    affiliationTicks = 0;
        //    affiliation = null;
        //}

        //public void GetAffiliatedCaravan()
        //{
        //    if (affiliation == null) return;
        //    if (affiliationTicks > Find.TickManager.TicksGame || affiliationTicks == 0) return;
        //    if (affiliation.HostileTo(Faction.OfPlayer))
        //    {
        //        BreakAffiliation(affiliation);
        //        return;
        //    }

        //    affiliationTicks = Find.TickManager.TicksGame + GenDate.TicksPerSeason + Rand.Range(-120000, 120000);

        //    IncidentParms incidentParms = new IncidentParms();
        //    incidentParms.target = parent.Map;
        //    incidentParms.faction = affiliation;
        //    incidentParms.traderKind = affiliation.def.caravanTraderKinds.RandomElement<TraderKindDef>();
        //    incidentParms.forced = true;
        //    Find.Storyteller.incidentQueue.Add(IncidentDefOf.TraderCaravanArrival, affiliationTicks, incidentParms);
        //}

        #endregion Affiliation

        #region Methods
        /// <summary>
        /// The force pool is where all fatigue and
        /// casting limits are decided.
        /// </summary>
        public Need_ForcePool ForcePool
        {
            get
            {
                return abilityUser.needs.TryGetNeed<Need_ForcePool>();
            }
        }

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
                            this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_GrayTrait) ||
                            this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_ForceSensitive))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        // RimWorld.TraitSet
        public void LoseTrait(TraitSet traits, Trait trait)
        {
            if (!traits.HasTrait(trait.def))
            {
                Log.Warning(this.abilityUser + " doesn't have trait " + trait.def);
                return;
            }
            traits.allTraits.Remove(trait);
            if (this.abilityUser.workSettings != null)
            {
                this.abilityUser.workSettings.Notify_GainedTrait();
            }
            //this.abilityUser.story.Notify_TraitChanged();
            if (this.abilityUser.skills != null)
            {
                this.abilityUser.skills.Notify_SkillDisablesChanged();
            }
            if (!this.abilityUser.Dead && this.abilityUser.RaceProps.Humanlike)
            {
                this.abilityUser.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }

        /// <summary>
        /// Shows the required alignment (optional), 
        /// alignment change (optional),
        /// and the force pool usage
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        public override string PostAbilityVerbCompDesc(VerbProperties_Ability verbDef)
        {
            string result = "";
            StringBuilder postDesc = new StringBuilder();
            ForceAbilityDef forceDef = (ForceAbilityDef)verbDef.abilityDef;
            if (forceDef != null)
            {
                string alignDesc = "";
                string changeDesc = "";
                string pointsDesc = "";
                if (forceDef.changedAlignmentType != ForceAlignmentType.None)
                {
                    alignDesc = "ForceAbilityDescAlign".Translate(new object[]
                    {
                    forceDef.requiredAlignmentType.ToString(),
                    });
                }
                if (forceDef.changedAlignmentType != ForceAlignmentType.None)
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
                if (forceDef.requiredAlignmentType != ForceAlignmentType.None)
                {
                    if (forceDef.requiredAlignmentType != this.ForceAlignmentType)
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

        public override void PostAbilityAttempt(Pawn caster, AbilityDef ability)
        {
            ForceAbilityDef forceDef = ability as ForceAbilityDef;
            if (forceDef != null)
            {
                if (forceDef.changedAlignmentType != ForceAlignmentType.None)
                {
                    //Log.Message("Alignment: " + AlignmentValue.ToStringPercent());
                    //Log.Message("Alignment Change: " + forceDef.changedAlignmentRate.ToStringPercent());
                    AlignmentValue += forceDef.changedAlignmentRate;
                    //Log.Message("New Alignment: " + AlignmentValue.ToStringPercent());
                    UpdateAlignment();
                }

                if (ForcePool != null)
                {
                    float poolCost = 0f;
                    //Log.Message("PC" + forceDef.forcePoolCost.ToString());
                    poolCost = forceDef.forcePoolCost - (forceDef.forcePoolCost * (0.15f * (float)ForceSkillLevel("PJ_ForcePool")));
                    //Log.Message("PC" + poolCost.ToString());
                    ForcePool.UseForcePower(poolCost);
                }
            }
        }
#endregion Methods

        #region Initialize
        public override void CompTick()
        {
            if (abilityUser != null)
            {
                if (abilityUser.Spawned)
                {
                    if (Find.TickManager.TicksGame > 200)
                    {
                        //if (Find.TickManager.TicksGame % 30 == 0)
                        //{
                        if (IsForceUser)
                        {
                            if (!firstTick) PostInitializeTick();
                            base.CompTick();
                            if (Find.TickManager.TicksGame % 30 == 0)
                            {
                                if (forceUserXP > ForceUserXPTillNextLevel) LevelUp();
                                //forceUserXP++;
                            }
                        }
                        //}
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
                        if (this.AlignmentValue == 0.0f) this.AlignmentValue = 0.5f;
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

            if (forcePowersInitialized) return;
            forcePowersInitialized = true;

            Trait jediTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            Trait sithTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            Trait grayTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            Trait sensitiveTrait = this.abilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);
            
            if (jediTrait != null)
            {
                switch (jediTrait.Degree)
                {
                    case 0:
                    case 1:
                        this.alignmentValue = 0.7f;
                        for (int o = 0; o < 2; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                        }
                        for (int i = 0; i < 1; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersLight.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 2:
                        this.alignmentValue = 0.8f;
                        for (int o = 0; o < 5; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersLight.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 3:
                        this.alignmentValue = 0.85f;
                        for (int o = 0; o < 8; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersLight.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 4:
                        this.alignmentValue = 0.99f;
                        for (int o = 0; o < 10; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersLight.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                }

                // !! DEBUG -- TO BE REMOVED LATER !!
            }
            if (grayTrait != null)
            {
                this.ForceAlignmentType = ForceAlignmentType.Gray; // Default to Gray
                this.alignmentValue = Rand.Range(0.4f, 0.6f);

                switch (grayTrait.Degree)
                {
                    case 0:
                    case 1:
                        for (int o = 0; o < 2; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }

                        for (int i = 0; i < 1; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersGray.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 2:
                        for (int o = 0; o < 3; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersGray.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 3:
                        for (int o = 0; o < 5; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersGray.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 4:
                        for (int o = 0; o < 10; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersGray.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 3));
                            this.abilityPoints -= 1;
                        }
                        return;
                }
            }

            if (sithTrait != null)
            {
                switch (sithTrait.Degree)
                {
                    case 0:
                    case 1:
                        this.alignmentValue = 0.3f;
                        for (int o = 0; o < 4; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 1; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersDark.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 2:
                        this.alignmentValue = 0.2f;
                        for (int o = 0; o < 5; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersDark.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 3:
                        this.alignmentValue = 0.15f;
                        for (int o = 0; o < 6; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersDark.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                    case 4:
                        this.alignmentValue = 0.0f;
                        for (int o = 0; o < 10; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceSkills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4).level++;
                            this.abilityPoints -= 1;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            this.ForceUserLevel += 1;
                            LevelUpPower(this.ForcePowersDark.InRandomOrder<ForcePower>().First((ForcePower x) => x.level < 2));
                            this.abilityPoints -= 1;
                        }
                        return;
                }
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
        #endregion Initialize

        #region ExposeData
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.LookValue<float>(ref this.alignmentValue, "alignmentValue", 0.5f);
            Scribe_Values.LookValue<int>(ref this.forceUserLevel, "forceUserLevel", 0);
            Scribe_Values.LookValue<int>(ref this.forceUserXP, "forceUserXP");
            Scribe_Values.LookValue<bool>(ref this.forcePowersInitialized, "forcePowersInitialized", false);
            //Scribe_Values.LookValue<int>(ref this.levelLightsaberOff, "levelLightsaberOff", 0);
            //Scribe_Values.LookValue<int>(ref this.levelLightsaberDef, "levelLightsaberDef", 0);
            //Scribe_Values.LookValue<int>(ref this.levelLightsaberAcc, "levelLightsaberAcc", 0);
            //Scribe_Values.LookValue<int>(ref this.levelLightsaberRef, "levelLightsaberRef", 0);
            //Scribe_Values.LookValue<int>(ref this.levelForcePool, "levelForcePool", 0);
            Scribe_Values.LookValue<int>(ref this.abilityPoints, "abilityPoints", 0);
            Scribe_Values.LookValue<int>(ref this.canMeditateTicks, "canMeditateTicks", 0);
            Scribe_Collections.LookList<ForcePower>(ref this.forcePowersDark, "forcePowersDark", LookMode.Deep, null);
            Scribe_Collections.LookList<ForcePower>(ref this.forcePowersGray, "forcePowersGray", LookMode.Deep, null);
            Scribe_Collections.LookList<ForcePower>(ref this.forcePowersLight, "forcePowersLight", LookMode.Deep, null);
            Scribe_Collections.LookList<ForceSkill>(ref this.forceSkills, "forceSkills", LookMode.Deep, null);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (ForcePowersDark != null && ForcePowersDark.Count > 0)
                {
                    foreach (ForcePower power in ForcePowersDark)
                    {
                        if (power.abilityDef != null)
                        {
                            if (power.level > 0)
                            {
                                this.AddPawnAbility(power.abilityDef);
                            }
                        }
                    }
                }

                if (ForcePowersGray != null && ForcePowersGray.Count > 0)
                {
                    foreach (ForcePower power in ForcePowersGray)
                    {
                        if (power.abilityDef != null)
                        {
                            if (power.level > 0)
                            {
                                this.AddPawnAbility(power.abilityDef);
                            }
                        }
                    }
                }

                if (ForcePowersLight != null && ForcePowersLight.Count > 0)
                {
                    foreach (ForcePower power in ForcePowersLight)
                    {
                        if (power.abilityDef != null)
                        {
                            if (power.level > 0)
                            {
                                this.AddPawnAbility(power.abilityDef);
                            }
                        }
                    }
                }

            }

            //Log.Message("PostExposeData Called: ForceUser");
        }
        #endregion ExposeData
    }
}
