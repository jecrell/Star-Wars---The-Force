using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using AbilityUser;

using RimWorld;

using UnityEngine;

using Verse;
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

        private ForceData forceData = null;

        public ForceData ForceData
        {
            get
            {
                if (this.forceData == null && this.IsForceUser)
                {
                    this.forceData = new ForceData(this);
                }

                return this.forceData;
            }
        }

        #endregion Variables

        // private List<ForceAbility> tempForceAbilities = null;
        // public List<ForceAbility> AllForceAbilities
        // {
        // get
        // {
        // if (tempForceAbilities == null)
        // {
        // tempForceAbilities = new List<ForceAbility>();
        // foreach (PawnAbility pa in AllPowers.ToList())
        // {
        // ForceAbility fa = new ForceAbility(pa.Pawn, pa.Def);
        // tempForceAbilities.Add(fa);
        // }
        // }
        // return tempForceAbilities;
        // }
        // }

        // public override IEnumerable<Gizmo> CompGetGizmosExtra()
        // {
        // for (int i = 0; i < this.ForceData.Powers.Count(); i++)
        // {
        // if (this.ForceData.Powers[i] is ForceAbility p) yield return p.GetGizmo();
        // }

        // }
        #region Levels

        public int ForceUserLevel
        {
            get
            {
                return this.ForceData.Level;
            }

            set
            {
                if (value > this.ForceData.Level)
                {
                    this.ForceData.AbilityPoints++;
                    if (this.ForceData.XP < value * 600)
                    {
                        this.ForceData.XP = value * 600;
                    }
                }

                this.ForceData.Level = value;
            }
        }

        public int ForceUserXP
        {
            get
            {
                return this.ForceData.XP;
            }

            set
            {
                this.ForceData.XP = value;
            }
        }

        public float XPLastLevel
        {
            get
            {
                float result = 0f;
                if (this.ForceUserLevel > 0) result = this.ForceUserLevel * 600;
                return result;
            }
        }

        public float XPTillNextLevelPercent
        {
            get
            {
                return (float)(this.ForceUserXP - this.XPLastLevel)
                       / (float)(this.ForceUserXPTillNextLevel - this.XPLastLevel);
            }
        }

        public int ForceUserXPTillNextLevel
        {
            get
            {
                return (this.ForceUserLevel + 1) * 600;
            }
        }

        public int ForceSkillLevel(string skillName)
        {
            int result = 0;
            ForceSkill skillCheck = this.ForceData.Skills.FirstOrDefault((ForceSkill x) => x.label == skillName);
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
                if (this.ForceData.PowersDark != null && this.ForceData.PowersDark.Count > 0)
                {
                    foreach (ForcePower power in this.ForceData.PowersDark)
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
                if (this.ForceData.PowersLight != null && this.ForceData.PowersLight.Count > 0)
                {
                    foreach (ForcePower power in this.ForceData.PowersLight)
                    {
                        result += power.level;
                    }
                }

                return result;
            }
        }

        public void LevelUp(bool hideNotification = false)
        {
            this.ForceUserLevel += 1;
            if (this.ForceUserLevel == 1)
            {
                if (!hideNotification)
                {
                    Messages.Message(
                        "PJ_ForcePowersUnlocked".Translate(new object[] { this.parent.Label }),
                        MessageTypeDefOf.SilentInput);
                    Find.LetterStack.ReceiveLetter(
                        "PJ_ForceAwakensLabel".Translate(),
                        "PJ_ForceAwakensDesc".Translate(new object[] { this.parent.Label }),
                        LetterDefOf.PositiveEvent,
                        this.parent,
                        null);
                }

                SoundDef.Named("PJ_ForcePowersUnlocked").PlayOneShotOnCamera();
                this.AlignmentValue = 0.5f;
            }
            else
            {
                if (!hideNotification)
                    Messages.Message(
                        "PJ_LevelUp".Translate(new object[] { this.parent.Label }),
                        MessageTypeDefOf.PositiveEvent);
            }

            // this.tempForceAbilities = null;
            this.UpdateAlignment();
        }

        public void ResetPowers()
        {
            foreach (ForceSkill skill in this.ForceData.Skills)
            {
                this.ForceData.AbilityPoints += skill.level;
                skill.level = 0;
            }

            foreach (ForcePower power in this.ForceData.PowersDark)
            {
                power.level = 0;
            }

            foreach (ForcePower power in this.ForceData.PowersGray)
            {
                power.level = 0;
            }

            foreach (ForcePower power in this.ForceData.PowersLight)
            {
                power.level = 0;
            }

            List<ForceAbility> tempList = new List<ForceAbility>();
            foreach (PawnAbility ab in this.AbilityData.Powers)
            {
                tempList.Add(ab as ForceAbility);
            }

            foreach (ForceAbility ability in tempList)
            {
                this.RemovePawnAbility(ability.Def);
            }

            tempList = null;

            this.ForceData.AbilityPoints = this.ForceUserLevel;
            // this.tempForceAbilities = null;
            this.UpdateAbilities();
        }

        public void LevelUpPower(ForcePower power)
        {
            foreach (AbilityUser.AbilityDef def in power.abilityDefs)
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
            // Change traits..
            Trait jediTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            Trait sithTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            Trait grayTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            Trait sensitiveTrait = this.AbilityUser.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);

            // Clear traits.
            if (jediTrait != null) this.LoseTrait(this.AbilityUser.story.traits, jediTrait);
            if (sithTrait != null) this.LoseTrait(this.AbilityUser.story.traits, sithTrait);
            if (grayTrait != null) this.LoseTrait(this.AbilityUser.story.traits, grayTrait);
            if (sensitiveTrait != null) this.LoseTrait(this.AbilityUser.story.traits, sensitiveTrait);

            // Jedi
            int degree = 0;

            if (this.ForceUserLevel > 14)
            {
                degree = 4;
            }
            else if (this.ForceUserLevel > 8)
            {
                degree = 3;
            }
            else if (this.ForceUserLevel > 3)
            {
                degree = 2;
            }
            else if (this.ForceUserLevel > 0)
            {
                degree = 1;
            }

            if (this.AlignmentValue > 0.75)
            {
                this.AbilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_JediTrait, degree, true));
                return;
            }

            // Gray
            else if (this.AlignmentValue >= 0.25 && this.AlignmentValue <= 0.75)
            {
                this.AbilityUser.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_GrayTrait, degree, true));
                return;
            }

            // Sith
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
                return this.ForceData.Alignment;
            }

            set
            {
                this.ForceData.Alignment = Mathf.Clamp(value, 0.0f, 1.0f);
                if (this.ForceUserLevel > 0) this.UpdateAlignment();
            }
        }

        public ForceAlignmentType ForceAlignmentType
        {
            get
            {
                if (this.ForceData.Alignment < 0.4) return ForceAlignmentType.Dark;
                if (this.ForceData.Alignment < 0.6) return ForceAlignmentType.Gray;
                return ForceAlignmentType.Light;
            }

            set
            {
                switch (value)
                {
                    case ForceAlignmentType.Dark:
                        this.ForceData.Alignment = 0.0f;
                        break;
                    case ForceAlignmentType.Gray:
                    case ForceAlignmentType.None:
                        this.ForceData.Alignment = 0.5f;
                        break;
                    case ForceAlignmentType.Light:
                        this.ForceData.Alignment = 1.0f;
                        break;
                }
            }
        }

        #endregion Alignment

        #region Affiliation

        // public void SetAffiliation(Faction newFaction)
        // {
        // affiliation = newFaction;
        // affiliationTicks = Find.TickManager.TicksGame + GenDate.TicksPerSeason + Rand.Range(-120000, 120000);
        // }

        // public void BreakAffiliation(Faction newFaction)
        // {
        // affiliationTicks = 0;
        // affiliation = null;
        // }

        // public void GetAffiliatedCaravan()
        // {
        // if (affiliation == null) return;
        // if (affiliationTicks > Find.TickManager.TicksGame || affiliationTicks == 0) return;
        // if (affiliation.HostileTo(Faction.OfPlayer))
        // {
        // BreakAffiliation(affiliation);
        // return;
        // }

        // affiliationTicks = Find.TickManager.TicksGame + GenDate.TicksPerSeason + Rand.Range(-120000, 120000);

        // IncidentParms incidentParms = new IncidentParms();
        // incidentParms.target = parent.Map;
        // incidentParms.faction = affiliation;
        // incidentParms.traderKind = affiliation.def.caravanTraderKinds.RandomElement<TraderKindDef>();
        // incidentParms.forced = true;
        // Find.Storyteller.incidentQueue.Add(IncidentDefOf.TraderCaravanArrival, affiliationTicks, incidentParms);
        // }
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
                return this.AbilityUser.needs.TryGetNeed<Need_ForcePool>();
            }
        }

        public bool IsForceUser => ForceUtility.IsForceUser(this.Pawn);

        public override bool TryTransformPawn()
        {
            return this.IsForceUser;
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
                this.AbilityUser.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
            }

            // this.AbilityUser.story.Notify_TraitChanged();
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
            if (this.AbilityUser != null)
            {
                if (this.AbilityUser.Spawned)
                {
                    if (Find.TickManager.TicksGame > 200)
                    {
                        // if (Find.TickManager.TicksGame % 30 == 0)
                        // {
                        if (this.IsForceUser)
                        {
                            if (!this.ForceData.ForcePowersInitialized)
                            {
                                this.PostInitializeTick();
                                this.ForceData.TabResolved = true;
                            }
                            else if (!this.ForceData.TabResolved)
                            {
                                this.ResolveForceTab();
                                this.ForceData.TabResolved = true;
                            }

                            base.CompTick();
                            if (Find.TickManager.TicksGame % 30 == 0)
                            {
                                if (this.ForceUserXP > this.ForceUserXPTillNextLevel) this.LevelUp();
                                // forceUserXP++;
                            }

                            ///Ticks for each ability
                            // if (!this.AllForceAbilities.NullOrEmpty())
                            // {
                            // foreach (ForceAbility power in this.AllForceAbilities)
                            // {
                            // power.Tick();
                            // }
                            // }
                        }

                        // }
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
                        this.ForceData.ForcePowersInitialized = true;
                        this.Initialize();
                        // if (ForceData.Alignment == 0.0f) ForceData.Alignment = 0.5f;
                        this.ResolveForceTab();
                        this.ResolveForcePowers();
                        this.ResolveForcePool();
                    }
                }
            }
        }

        public void ResolveForceTab()
        {
            // PostExposeData();
            // Make the ITab
            IEnumerable<InspectTabBase> tabs = this.AbilityUser.GetInspectTabs();
            if (tabs != null && tabs.Count<InspectTabBase>() > 0)
            {
                if (tabs.FirstOrDefault((InspectTabBase x) => x is ITab_Pawn_Force) == null)
                {
                    try
                    {
                        this.AbilityUser.def.inspectorTabsResolved.Add(
                            InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Force)));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(
                            string.Concat(
                                new object[]
                                    {
                                        "Could not instantiate inspector tab of type ", typeof(ITab_Pawn_Force), ": ",
                                        ex
                                    }));
                    }
                }
            }
        }

        public void ResolveForcePowers()
        {
            // Set the force alignment
            if (this.ForceData.forcePowersInitialized) return;
            this.ForceData.forcePowersInitialized = true;

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
                        this.ForceData.Alignment = 0.7f;
                        for (int o = 0; o < 2; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                        }

                        for (int i = 0; i < 1; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersLight.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 2:
                        this.ForceData.Alignment = 0.8f;
                        for (int o = 0; o < 5; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersLight.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 3:
                        this.ForceData.Alignment = 0.85f;
                        for (int o = 0; o < 8; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersLight.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 4:
                        this.ForceData.Alignment = 0.99f;
                        for (int o = 0; o < 10; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersLight.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                }

                // !! DEBUG -- TO BE REMOVED LATER !!
            }

            if (grayTrait != null)
            {
                this.ForceAlignmentType = ForceAlignmentType.Gray; // Default to Gray
                this.ForceData.Alignment = Rand.Range(0.4f, 0.6f);

                switch (grayTrait.Degree)
                {
                    case 0:
                    case 1:
                        for (int o = 0; o < 2; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 1; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersGray.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 2:
                        for (int o = 0; o < 3; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersGray.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 3:
                        for (int o = 0; o < 5; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersGray.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 4:
                        for (int o = 0; o < 10; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersGray.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 3));
                            this.ForceData.AbilityPoints -= 1;
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
                        this.ForceData.Alignment = 0.3f;
                        for (int o = 0; o < 4; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 1; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersDark.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 2:
                        this.ForceData.Alignment = 0.2f;
                        for (int o = 0; o < 5; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersDark.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 3:
                        this.ForceData.Alignment = 0.15f;
                        for (int o = 0; o < 6; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersDark.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 4:
                        this.ForceData.Alignment = 0.0f;
                        for (int o = 0; o < 10; o++)
                        {
                            this.ForceUserLevel += 1;
                            this.ForceData.Skills.InRandomOrder<ForceSkill>().First((ForceSkill x) => x.level < 4)
                                .level++;
                            this.ForceData.AbilityPoints -= 1;
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            this.ForceUserLevel += 1;
                            this.LevelUpPower(
                                this.ForceData.PowersDark.InRandomOrder<ForcePower>()
                                    .First((ForcePower x) => x.level < 2));
                            this.ForceData.AbilityPoints -= 1;
                        }

                        return;
                }
            }
        }

        public void ResolveForcePool()
        {
            // Add the hediff if no pool exists.
            if (this.ForcePool == null)
            {
                Hediff forceWielderHediff =
                    this.AbilityUser.health.hediffSet.GetFirstHediffOfDef(ProjectJediDefOf.PJ_ForceWielderHD);
                if (forceWielderHediff != null)
                {
                    forceWielderHediff.Severity = 1.0f;
                }
                else
                {
                    Hediff newForceWielderHediff =
                        HediffMaker.MakeHediff(ProjectJediDefOf.PJ_ForceWielderHD, this.AbilityUser, null);
                    newForceWielderHediff.Severity = 1.0f;
                    this.AbilityUser.health.AddHediff(newForceWielderHediff, null, null);
                }
            }
        }

        #endregion Initialize

        #region ExposeData

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<ForceData>(ref this.forceData, "forceData", new object[] { this });

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                var abilities = new List<ForceAbility>();
                if (this.ForceData != null && ForceData.Powers.Count() > 0)
                {
                    foreach (ForcePower power in ForceData.Powers)
                    {
                        if (power.abilityDef != null)
                        {
                            if (power.level > 0)
                            {
                                if (this.AbilityData.Powers.FirstOrDefault(x => x.Def == power.abilityDef) == null)
                                    this.AddPawnAbility(power.abilityDef, true, power.ticksUntilNextCast);
                            }
                        }
                    }
                }
                #endregion ExposeData
            }
        }
    }
}
