using System;
using System.Collections.Generic;
using System.Linq;
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
        private ForceData forceData;

        public ForceData ForceData
        {
            get
            {
                if (forceData == null && IsForceUser)
                {
                    forceData = new ForceData(this);
                }

                return forceData;
            }
        }

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

        public int ForceUserLevel
        {
            get => ForceData.Level;

            set
            {
                if (value > ForceData.Level)
                {
                    ForceData.AbilityPoints++;
                    if (ForceData.XP < value * 600)
                    {
                        ForceData.XP = value * 600;
                    }
                }

                ForceData.Level = value;
            }
        }

        public int ForceUserXP
        {
            get => ForceData.XP;

            set => ForceData.XP = value;
        }

        public float XPLastLevel
        {
            get
            {
                var result = 0f;
                if (ForceUserLevel > 0)
                {
                    result = ForceUserLevel * 600;
                }

                return result;
            }
        }

        public float XPTillNextLevelPercent =>
            (ForceUserXP - XPLastLevel)
            / (ForceUserXPTillNextLevel - XPLastLevel);

        public int ForceUserXPTillNextLevel => (ForceUserLevel + 1) * 600;

        public int DarksidePoints
        {
            get
            {
                var result = 0;
                if (ForceData.PowersDark == null || ForceData.PowersDark.Count <= 0)
                {
                    return result;
                }

                foreach (var power in ForceData.PowersDark)
                {
                    result += power.level;
                }

                return result;
            }
        }

        public int LightsidePoints
        {
            get
            {
                var result = 0;
                if (ForceData.PowersLight == null || ForceData.PowersLight.Count <= 0)
                {
                    return result;
                }

                foreach (var power in ForceData.PowersLight)
                {
                    result += power.level;
                }

                return result;
            }
        }

        /// <summary>
        ///     Keep track of an internal alignment.
        ///     As a float value, this allows greater roleplaying possibilities.
        /// </summary>
        public float AlignmentValue
        {
            get => ForceData.Alignment;

            set
            {
                ForceData.Alignment = Mathf.Clamp(value, 0.0f, 1.0f);
                if (ForceUserLevel > 0)
                {
                    UpdateAlignment();
                }
            }
        }

        public ForceAlignmentType ForceAlignmentType
        {
            get
            {
                if (ForceData.Alignment < 0.4)
                {
                    return ForceAlignmentType.Dark;
                }

                if (ForceData.Alignment < 0.6)
                {
                    return ForceAlignmentType.Gray;
                }

                return ForceAlignmentType.Light;
            }

            set
            {
                switch (value)
                {
                    case ForceAlignmentType.Dark:
                        ForceData.Alignment = 0.0f;
                        break;
                    case ForceAlignmentType.Gray:
                    case ForceAlignmentType.None:
                        ForceData.Alignment = 0.5f;
                        break;
                    case ForceAlignmentType.Light:
                        ForceData.Alignment = 1.0f;
                        break;
                }
            }
        }

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

        /// <summary>
        ///     The force pool is where all fatigue and
        ///     casting limits are decided.
        /// </summary>
        public Need_ForcePool ForcePool => Pawn.needs.TryGetNeed<Need_ForcePool>();

        public bool IsForceUser => Pawn.IsForceUser();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref forceData, "forceData", this);

            if (Scribe.mode != LoadSaveMode.PostLoadInit)
            {
                return;
            }

            var unused = new List<ForceAbility>();
            if (ForceData == null || !ForceData.Powers.Any())
            {
                return;
            }

            foreach (var power in ForceData.Powers)
            {
                if (power.AbilityDef == null)
                {
                    continue;
                }

                if (power.level <= 0)
                {
                    continue;
                }

                if (AbilityData.Powers.FirstOrDefault(x => x.Def == power.AbilityDef) == null)
                {
                    AddPawnAbility(power.AbilityDef, true, power.ticksUntilNextCast);
                }
            }
        }

        public int ForceSkillLevel(string skillName)
        {
            var result = 0;
            var skillCheck = ForceData.Skills.FirstOrDefault(x => x.label == skillName);
            if (skillCheck != null)
            {
                result = skillCheck.level;
            }

            return result;
        }

        public void LevelUp(bool hideNotification = false)
        {
            ForceUserLevel += 1;
            if (ForceUserLevel == 1)
            {
                if (!hideNotification)
                {
                    Messages.Message(
                        "PJ_ForcePowersUnlocked".Translate(parent.Label),
                        MessageTypeDefOf.SilentInput);
                    Find.LetterStack.ReceiveLetter(
                        label: "PJ_ForceAwakensLabel".Translate(),
                        text: "PJ_ForceAwakensDesc".Translate(parent.Label),
                        textLetterDef: LetterDefOf.PositiveEvent,
                        lookTargets: parent
                    );
                }

                SoundDef.Named("PJ_ForcePowersUnlocked").PlayOneShotOnCamera();
                AlignmentValue = 0.5f;
            }
            else
            {
                if (!hideNotification)
                {
                    Messages.Message(
                        text: "PJ_LevelUp".Translate(parent.Label),
                        lookTargets: parent,
                        def: MessageTypeDefOf.PositiveEvent
                    );
                }
            }

            // this.tempForceAbilities = null;
            UpdateAlignment();
        }

        public void ResetPowers()
        {
            foreach (var skill in ForceData.Skills)
            {
                ForceData.AbilityPoints += skill.level;
                skill.level = 0;
            }

            foreach (var power in ForceData.PowersDark)
            {
                power.level = 0;
            }

            foreach (var power in ForceData.PowersGray)
            {
                power.level = 0;
            }

            foreach (var power in ForceData.PowersLight)
            {
                power.level = 0;
            }

            var tempList = new List<ForceAbility>();
            foreach (var ab in AbilityData.Powers)
            {
                tempList.Add(ab as ForceAbility);
            }

            foreach (var ability in tempList)
            {
                RemovePawnAbility(ability.Def);
            }

            ForceData.AbilityPoints = ForceUserLevel;
            // this.tempForceAbilities = null;
            UpdateAbilities();
        }

        public void LevelUpPower(ForcePower power)
        {
            foreach (var def in power.abilityDefs)
            {
                RemovePawnAbility(def);
            }

            power.level++;
            AddPawnAbility(power.AbilityDef);
        }

        /// <summary>
        ///     Updates the alignment after a level up or force power casting
        /// </summary>
        public void UpdateAlignment()
        {
            // Change traits..
            var jediTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            var sithTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            var grayTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            var sensitiveTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);

            // Clear traits.
            if (jediTrait != null)
            {
                LoseTrait(Pawn.story.traits, jediTrait);
            }

            if (sithTrait != null)
            {
                LoseTrait(Pawn.story.traits, sithTrait);
            }

            if (grayTrait != null)
            {
                LoseTrait(Pawn.story.traits, grayTrait);
            }

            if (sensitiveTrait != null)
            {
                LoseTrait(Pawn.story.traits, sensitiveTrait);
            }

            // Jedi
            var degree = 0;

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
                Pawn.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_JediTrait, degree, true));
            }

            // Gray
            else if (AlignmentValue >= 0.25 && AlignmentValue <= 0.75)
            {
                Pawn.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_GrayTrait, degree, true));
            }

            // Sith
            else
            {
                Pawn.story.traits.GainTrait(new Trait(ProjectJediDefOf.PJ_SithTrait, degree, true));
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
                Log.Warning(Pawn + " doesn't have trait " + trait.def);
                return;
            }

            traits.allTraits.Remove(trait);
            if (Pawn.workSettings != null)
            {
                Pawn.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
            }

            // this.Pawn.story.Notify_TraitChanged();
            if (Pawn.skills != null)
            {
                Pawn.skills.Notify_SkillDisablesChanged();
            }

            if (!Pawn.Dead && Pawn.RaceProps.Humanlike)
            {
                Pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }

        public override List<HediffDef> IgnoredHediffs()
        {
            var newDefs = new List<HediffDef>
            {
                ProjectJediDefOf.PJ_ForceWielderHD
            };
            return newDefs;
        }

        public override void CompTick()
        {
            if (Pawn == null)
            {
                return;
            }

            if (!Pawn.Spawned)
            {
                return;
            }

            if (Find.TickManager.TicksGame <= 200)
            {
                return;
            }

            // if (Find.TickManager.TicksGame % 30 == 0)
            // {
            if (!IsForceUser)
            {
                return;
            }

            if (!ForceData.ForcePowersInitialized)
            {
                PostInitializeTick();
                ForceData.TabResolved = true;
            }
            else if (!ForceData.TabResolved)
            {
                ResolveForceTab();
                ForceData.TabResolved = true;
            }


            base.CompTick();
            if (Find.TickManager.TicksGame % 30 != 0)
            {
                return;
            }

            //It's important to allow for concepts to be shared on how this mod works.
            if (ModInfo.TutorLesson_Sensitive && Pawn?.story?.traits?.HasTrait(ProjectJediDefOf.PJ_ForceSensitive) == true)
            {
                LessonAutoActivator.TeachOpportunity(ConceptDef.Named("PJ_TutorForceSensitive"), Pawn, OpportunityType.Important);
            }

            if (ModInfo.TutorLesson_OtherForce)
            {
                LessonAutoActivator.TeachOpportunity(ConceptDef.Named("PJ_TutorForceXP"), Pawn, OpportunityType.Important);
                LessonAutoActivator.TeachOpportunity(ConceptDef.Named("PJ_TutorForceAlignment"), Pawn, OpportunityType.Important);
            }
            

            if (ForceUserXP > ForceUserXPTillNextLevel)
            {
                LevelUp();
            }
            // forceUserXP++;

            //Ticks for each ability
            // if (!this.AllForceAbilities.NullOrEmpty())
            // {
            // foreach (ForceAbility power in this.AllForceAbilities)
            // {
            // power.Tick();
            // }
            // }

            // }
        }

        /// <summary>
        ///     Creates a force user by adding a hidden Hediff that adds their Force Pool needs.
        /// </summary>
        public void PostInitializeTick()
        {
            if (Pawn == null)
            {
                return;
            }

            if (!Pawn.Spawned)
            {
                return;
            }

            if (Pawn.story == null)
            {
                return;
            }

            ForceData.ForcePowersInitialized = true;
            Initialize();
            // if (ForceData.Alignment == 0.0f) ForceData.Alignment = 0.5f;
            ResolveForceTab();
            ResolveForcePowers();
            ResolveForcePool();
        }

        public void ResolveForceTab()
        {
            // PostExposeData();
            // Make the ITab
            var tabs = Pawn.GetInspectTabs();
            if (tabs != null && !tabs.Any())
            {
                return;
            }

            if (tabs!.FirstOrDefault(x => x is ITab_Pawn_Force) != null)
            {
                return;
            }

            try
            {
                Pawn.def.inspectorTabsResolved.Add(
                    InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Force)));
            }
            catch (Exception ex)
            {
                Log.Error(
                    string.Concat("Could not instantiate inspector tab of type ", typeof(ITab_Pawn_Force), ": ",
                        ex));
            }
        }

        public void ResolveForcePowers()
        {
            // Set the force alignment
            if (ForceData.forcePowersInitialized)
            {
                return;
            }

            ForceData.forcePowersInitialized = true;

            var jediTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            var sithTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            var grayTrait = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);
            var unused = Pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);

            if (jediTrait != null)
            {
                switch (jediTrait.Degree)
                {
                    case 0:
                    case 1:
                        ForceData.Alignment = 0.7f;
                        for (var o = 0; o < 2; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                        }

                        for (var i = 0; i < 1; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersLight.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 2:
                        ForceData.Alignment = 0.8f;
                        for (var o = 0; o < 5; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 3; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersLight.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 3:
                        ForceData.Alignment = 0.85f;
                        for (var o = 0; o < 8; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 6; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersLight.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 4:
                        ForceData.Alignment = 0.99f;
                        for (var o = 0; o < 10; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 8; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersLight.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                }

                // !! DEBUG -- TO BE REMOVED LATER !!
            }

            if (grayTrait != null)
            {
                ForceAlignmentType = ForceAlignmentType.Gray; // Default to Gray
                ForceData.Alignment = Rand.Range(0.4f, 0.6f);

                switch (grayTrait.Degree)
                {
                    case 0:
                    case 1:
                        for (var o = 0; o < 2; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 1; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersGray.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 2:
                        for (var o = 0; o < 3; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 3; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersGray.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 3:
                        for (var o = 0; o < 5; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 6; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersGray.InRandomOrder()
                                    .First(x => x.level < 2));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                    case 4:
                        for (var o = 0; o < 10; o++)
                        {
                            ForceUserLevel += 1;
                            ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                                .level++;
                            ForceData.AbilityPoints -= 1;
                        }

                        for (var i = 0; i < 8; i++)
                        {
                            ForceUserLevel += 1;
                            LevelUpPower(
                                ForceData.PowersGray.InRandomOrder()
                                    .First(x => x.level < 3));
                            ForceData.AbilityPoints -= 1;
                        }

                        return;
                }
            }

            if (sithTrait == null)
            {
                return;
            }

            switch (sithTrait.Degree)
            {
                case 0:
                case 1:
                    ForceData.Alignment = 0.3f;
                    for (var o = 0; o < 4; o++)
                    {
                        ForceUserLevel += 1;
                        ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                            .level++;
                        ForceData.AbilityPoints -= 1;
                    }

                    for (var i = 0; i < 1; i++)
                    {
                        ForceUserLevel += 1;
                        LevelUpPower(
                            ForceData.PowersDark.InRandomOrder()
                                .First(x => x.level < 2));
                        ForceData.AbilityPoints -= 1;
                    }

                    return;
                case 2:
                    ForceData.Alignment = 0.2f;
                    for (var o = 0; o < 5; o++)
                    {
                        ForceUserLevel += 1;
                        ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                            .level++;
                        ForceData.AbilityPoints -= 1;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        ForceUserLevel += 1;
                        LevelUpPower(
                            ForceData.PowersDark.InRandomOrder()
                                .First(x => x.level < 2));
                        ForceData.AbilityPoints -= 1;
                    }

                    return;
                case 3:
                    ForceData.Alignment = 0.15f;
                    for (var o = 0; o < 6; o++)
                    {
                        ForceUserLevel += 1;
                        ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                            .level++;
                        ForceData.AbilityPoints -= 1;
                    }

                    for (var i = 0; i < 6; i++)
                    {
                        ForceUserLevel += 1;
                        LevelUpPower(
                            ForceData.PowersDark.InRandomOrder()
                                .First(x => x.level < 2));
                        ForceData.AbilityPoints -= 1;
                    }

                    return;
                case 4:
                    ForceData.Alignment = 0.0f;
                    for (var o = 0; o < 10; o++)
                    {
                        ForceUserLevel += 1;
                        ForceData.Skills.InRandomOrder().First(x => x.level < 4)
                            .level++;
                        ForceData.AbilityPoints -= 1;
                    }

                    for (var i = 0; i < 8; i++)
                    {
                        ForceUserLevel += 1;
                        LevelUpPower(
                            ForceData.PowersDark.InRandomOrder()
                                .First(x => x.level < 2));
                        ForceData.AbilityPoints -= 1;
                    }

                    return;
            }
        }

        public void ResolveForcePool()
        {
            // Add the hediff if no pool exists.
            if (ForcePool != null)
            {
                return;
            }

            var forceWielderHediff =
                Pawn.health.hediffSet.GetFirstHediffOfDef(ProjectJediDefOf.PJ_ForceWielderHD);
            if (forceWielderHediff != null)
            {
                forceWielderHediff.Severity = 1.0f;
            }
            else
            {
                var newForceWielderHediff =
                    HediffMaker.MakeHediff(ProjectJediDefOf.PJ_ForceWielderHD, Pawn);
                newForceWielderHediff.Severity = 1.0f;
                Pawn.health.AddHediff(newForceWielderHediff);
            }
        }
    }
}