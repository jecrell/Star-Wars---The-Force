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

        /// <summary>
        /// Keep track of an internal alignment.
        /// As a float value, this allows greater roleplaying possibilities.
        /// </summary>
        private float alignmentValue;

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

        /// <summary>
        /// Creates a force user by adding a hidden Hediff that adds their Force Pool needs.
        /// </summary>
        public override void PostInitialize()
        {

            //Set the force alignment
            this.ForceAlignmentType = ForceAlignmentType.Gray; // Default to Gray
            if (this.abilityUser != null)
            {
                if (this.abilityUser.story != null)
                {
                    if (this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait))
                    {
                        this.ForceAlignmentType = ForceAlignmentType.Light;

                        // !! DEBUG -- TO BE REMOVED LATER !!
                        this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceHealingSelf);
                    }
                    if (this.abilityUser.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait))
                    {
                        this.ForceAlignmentType = ForceAlignmentType.Dark;

                        // !! DEBUG -- TO BE REMOVED LATER !!
                        this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_ForceDrain);
                        this.abilityPowerManager.AddPawnAbility(ProjectJedi.ProjectJediDefOf.PJ_Lightning);
                    }
                }
            }

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
            Scribe_Values.LookValue<float>(ref alignmentValue, "alignmentValue", 0.0f);
        }
    }
}
