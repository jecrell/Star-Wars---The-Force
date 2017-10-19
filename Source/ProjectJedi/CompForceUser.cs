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
        public int ticksToLearnForceXP = -1;
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

        //private List<ForceAbility> tempForceAbilities = null;
        //public List<ForceAbility> AllForceAbilities
        //{
        //    get
        //    {
        //        if (tempForceAbilities == null)
        //        {
        //            tempForceAbilities = new List<ForceAbility>();
        //            foreach (PawnAbility pa in AllPowers.ToList())
        //            {
        //                ForceAbility fa = new ForceAbility(pa.Pawn, pa.Def);
        //                tempForceAbilities.Add(fa);
        //            }
        //        }
        //        return tempForceAbilities;
        //    }
        //}

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            for (int i = 0; i < this.AllPowers.Count; i++)
            {
                if (this.AllPowers[i] is ForceAbility p) yield return p.GetGizmo();
            }

        }

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
                        }), LetterDefOf.Good, this.parent, null);
                }
                SoundDef.Named("PJ_ForcePowersUnlocked").PlayOneShotOnCamera();
                AlignmentValue = 0.5f;
            }
            else
            {
                if (!hideNotification) Messages.Message("PJ_LevelUp".Translate(new object[]
                {
                this.parent.Label
                }), MessageSound.Benefit);
            }
            //this.tempForceAbilities = null;
            UpdateAlignment();
        }
        public void ResetPowers()
        {
            foreach (ForceSkill skill in ForceSkills)
            {
                this.abilityPoints += skill.level;
                skill.level = 0;
            }
            foreach (ForcePower power in ForcePowersDark)
            {
                power.level = 0;
            }
            foreach (ForcePower power in ForcePowersGray)
            {
                power.level = 0;
            }
            foreach (ForcePower power in ForcePowersLight)
            {
                power.level = 0;
            }

            List<ForceAbility> tempList = new List<ForceAbility>();
            foreach (PawnAbility ab in this.Powers)
            {
                tempList.Add(ab as ForceAbility);
            }
            foreach (ForceAbility ability in tempList)
            {
                this.RemovePawnAbility(ability.Def);
            }
            tempList = null;

            this.abilityPoints = this.forceUserLevel;
            //this.tempForceAbilities = null;
            UpdateAbilities();
        }

        public void LevelUpPower(ForcePower power)
        {
            foreach (AbilityDef def in power.abilityDefs)
            {
                this.RemovePawnAbility(def);
            }
            power.level++;
            this.AddPawnAbility(power.abilityDef);
        }
        /// <summary>
        /// Updates the alignment after a level up or force power casting
        /// </summary>
        public void UpdateAlignment()
        {
            //Change traits...
            Trait jediTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            Trait sithTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            Trait grayTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            Trait sensitiveTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);

            //Clear traits.
            if (jediTrait != null) LoseTrait(this.AbilityUser.story.traits, jediTrait);
            if (sithTrait != null) LoseTrait(this.AbilityUser.story.traits, sithTrait);
            if (grayTrait != null) LoseTrait(this.AbilityUser.story.traits, grayTrait);
            if (sensitiveTrait != null) LoseTrait(this.AbilityUser.story.traits, sensitiveTrait);

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
                this.AbilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_JediTrait, degree, true));
                return;
            }
            //Gray
            else if (AlignmentValue >= 0.25 && AlignmentValue <= 0.75)
            {
                this.AbilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_GrayTrait, degree, true));
                return;
            }
            //Sith
            else
            {
                this.AbilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_SithTrait, degree, true));
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
                return AbilityUser.needs.TryGetNeed<Need_ForcePool>();
            }
        }

        public bool IsForceUser
        {
            get
            {
                if (this.AbilityUser != null)
                {
                    if (this.AbilityUser is PawnGhost) return true;
                    if (this.AbilityUser.story != null)
                    {
                        if (this.AbilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                            this.AbilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait) ||
                            this.AbilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_GrayTrait) ||
                            this.AbilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_ForceSensitive))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        

        public override bool TryTransformPawn()
        {
            return IsForceUser;
        }


        // RimWorld.TraitSet
        public void LoseTrait(TraitSet traits, Trait trait)
        {
            if (!traits.HasTrait(trait.def))
            {
                Log.Warning(this.AbilityUser + " doesn't have trait " + trait.def);
                return;
            }
            traits.allTraits.Remove(trait);
            if (this.AbilityUser.workSettings != null)
            {
                this.AbilityUser.workSettings.Notify_GainedTrait();
            }
            //this.AbilityUser.story.Notify_TraitChanged();
            if (this.AbilityUser.skills != null)
            {
                this.AbilityUser.skills.Notify_SkillDisablesChanged();
            }
            if (!this.AbilityUser.Dead && this.AbilityUser.RaceProps.Humanlike)
            {
                this.AbilityUser.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }





        public override List<HediffDef> IgnoredHediffs()
        {
            List<HediffDef> newDefs = new List<HediffDef>();
            newDefs.Add(ProjectJediDefOf.PJ_ForceWielderHD);
            return newDefs;
        }



#endregion Methods

        #region Initialize
        public override void CompTick()
        {
            if (AbilityUser != null)
            {
                if (AbilityUser.Spawned)
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
                            
                            ///Ticks for each ability
                            //if (!this.AllForceAbilities.NullOrEmpty())
                            //{
                            //    foreach (ForceAbility power in this.AllForceAbilities)
                            //    {
                            //        power.Tick();
                            //    }
                            //}
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
            if (this.AbilityUser != null)
            {
                if (this.AbilityUser.Spawned)
                {
                    if (this.AbilityUser.story != null)
                    {
                        firstTick = true;
                        this.Initialize();
                        //if (this.AlignmentValue == 0.0f) this.AlignmentValue = 0.5f;
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
            IEnumerable<InspectTabBase> tabs = this.AbilityUser.GetInspectTabs();
            if (tabs != null && tabs.Count<InspectTabBase>() > 0)
            {
                if (tabs.FirstOrDefault((InspectTabBase x) => x is ITab_Pawn_Force) == null)
                {
                    try
                    {
                        this.AbilityUser.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Force)));
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

            Trait jediTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            Trait sithTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            Trait grayTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            Trait sensitiveTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);
            
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
                Hediff forceWielderHediff = AbilityUser.health.hediffSet.GetFirstHediffOfDef(ProjectJediDefOf.PJ_ForceWielderHD);
                if (forceWielderHediff != null)
                {
                    forceWielderHediff.Severity = 1.0f;
                }
                else
                {
                    Hediff newForceWielderHediff = HediffMaker.MakeHediff(ProjectJediDefOf.PJ_ForceWielderHD, AbilityUser, null);
                    newForceWielderHediff.Severity = 1.0f;
                    AbilityUser.health.AddHediff(newForceWielderHediff, null, null);
                }
            }
        }
        #endregion Initialize

        #region ExposeData
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.alignmentValue, "alignmentValue", 0.5f);
            Scribe_Values.Look<int>(ref this.forceUserLevel, "forceUserLevel", 0);
            Scribe_Values.Look<int>(ref this.forceUserXP, "forceUserXP");
            Scribe_Values.Look<bool>(ref this.forcePowersInitialized, "forcePowersInitialized", false);
            Scribe_Values.Look<int>(ref this.abilityPoints, "abilityPoints", 0);
            Scribe_Values.Look<int>(ref this.canMeditateTicks, "canMeditateTicks", 0);
            Scribe_Values.Look<int>(ref this.ticksToLearnForceXP, "ticksToLearnForceXP", -1);
            Scribe_Collections.Look<ForcePower>(ref this.forcePowersDark, "forcePowersDark", LookMode.Deep, new object[0]);
            Scribe_Collections.Look<ForcePower>(ref this.forcePowersGray, "forcePowersGray", LookMode.Deep, new object[0]);
            Scribe_Collections.Look<ForcePower>(ref this.forcePowersLight, "forcePowersLight", LookMode.Deep, new object[0]);
            Scribe_Collections.Look<ForceSkill>(ref this.forceSkills, "forceSkills", LookMode.Deep, new object[0]);

            if (Scribe.mode == LoadSaveMode.Saving)
            {

                if (!ForcePowersDark.NullOrEmpty())
                {
                    foreach (ForcePower power in ForcePowersDark)
                    {
                        if (power.abilityDef != null)
                        {
                            if (!Powers.NullOrEmpty() && Powers.FirstOrDefault(x => x.Def == power.abilityDef) is ForceAbility listPower)
                            {
                                power.ticksUntilNextCast = listPower.CooldownTicksLeft;
                            }
                        }
                    }
                }

                if (!ForcePowersGray.NullOrEmpty())
                {
                    foreach (ForcePower power in ForcePowersGray)
                    {
                        if (power.abilityDef != null)
                        {
                            if (!Powers.NullOrEmpty() && Powers.FirstOrDefault(x => x.Def == power.abilityDef) is ForceAbility listPower)
                            {
                                power.ticksUntilNextCast = listPower.CooldownTicksLeft;
                            }
                        }
                    }
                }

                if (!ForcePowersLight.NullOrEmpty())
                {
                    foreach (ForcePower power in ForcePowersLight)
                    {
                        if (power.abilityDef != null)
                        {
                            if (!Powers.NullOrEmpty() && Powers.FirstOrDefault(x => x.Def == power.abilityDef) is ForceAbility listPower)
                            {
                                power.ticksUntilNextCast = listPower.CooldownTicksLeft;
                            }
                        }
                    }
                }
            //}

            //if (Scribe.mode == LoadSaveMode.PostLoadInit)
            //{
            //    var abilities = new List<ForceAbility>();
            //    if (!this.Powers.NullOrEmpty())
            //    {
            //        foreach (PawnAbility ab in this.Powers)
            //        {
            //            abilities.Add(ab as ForceAbility);
            //        }
            //        if (!abilities.NullOrEmpty())
            //        {
            //            foreach (ForceAbility pab in abilities)
            //            {
            //                this.RemovePawnAbility(pab.Def);
            //            }
            //        }
            //    }

            //    if (!ForcePowersDark.NullOrEmpty())
            //    {
            //        foreach (ForcePower power in ForcePowersDark)
            //        {
            //            if (power.abilityDef != null)
            //            {
            //                if (power.level > 0)
            //                {
            //                    this.AddPawnAbility(power.abilityDef, true, power.ticksUntilNextCast);
            //                }
            //            }
            //        }
            //    }

            //    if (!ForcePowersGray.NullOrEmpty())
            //    {
            //        foreach (ForcePower power in ForcePowersGray)
            //        {
            //            if (power.abilityDef != null)
            //            {
            //                if (power.level > 0)
            //                {
            //                    this.AddPawnAbility(power.abilityDef, true, power.ticksUntilNextCast);
            //                }
            //            }
            //        }
            //    }

            //    if (!ForcePowersLight.NullOrEmpty())
            //    {
            //        foreach (ForcePower power in ForcePowersLight)
            //        {
            //            if (power.abilityDef != null)
            //            {
            //                if (power.level > 0)
            //                {
            //                    this.AddPawnAbility(power.abilityDef, true, power.ticksUntilNextCast);
            //                }
            //            }
            //        }
            //    }

            }

            //Log.Message("PostExposeData Called: ForceUser");
        }
        #endregion ExposeData
    }
}
