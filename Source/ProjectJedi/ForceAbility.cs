using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using AbilityUser;
using UnityEngine;

namespace ProjectJedi
{
    public class ForceAbility : PawnAbility
    {
        public CompForceUser ForceUser => ForceUtility.GetForceUser(this.Pawn);
        public ForceAbilityDef ForceDef => Def as ForceAbilityDef;

        public ForceAbility() : base()
        {
        }
        
        public ForceAbility(CompAbilityUser abilityUser) : base(abilityUser)
        {
            this.abilityUser = abilityUser as CompForceUser;
        }


        public ForceAbility(AbilityData abilityData) : base(abilityData)
        {
            this.abilityUser = abilityData.Pawn.AllComps.FirstOrDefault(x => x.GetType() == abilityData.AbilityClass) as CompForceUser;
        }


        public ForceAbility(Pawn user, AbilityDef pdef) : base(user, pdef)
        {

        }

        private float ActualForceCost => ForceDef.forcePoolCost - (ForceDef.forcePoolCost * (0.15f * (float)ForceUser.ForceSkillLevel("PJ_ForcePool")));
        
        public override void PostAbilityAttempt()
        {
            //Log.Message("ForceAbility :: PostAbilityAttempt Called");
            base.PostAbilityAttempt();
            if (ForceDef.changedAlignmentType != ForceAlignmentType.None)
            {
                ForceUser.AlignmentValue += ForceDef.changedAlignmentRate;
                ForceUser.UpdateAlignment();
            }
            Pawn.needs.TryGetNeed<Need_ForcePool>().UseForcePower(ActualForceCost);
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
            //Log.Message("1");
            string result = "";
            if (verbDef == null) return result;
            if (verbDef?.abilityDef is ForceAbilityDef forceDef)
            {
                //Log.Message("2");

                StringBuilder postDesc = new StringBuilder();
                string alignDesc = "";
                string changeDesc = "";
                string pointsDesc = "";
                //Log.Message("3");

                if (forceDef?.changedAlignmentType != ForceAlignmentType.None)
                {
                    //Log.Message("3a");

                    alignDesc = "ForceAbilityDescAlign".Translate(new object[]
                    {
                    forceDef.requiredAlignmentType.ToString(),
                    });
                }
                //Log.Message("4");

                if (forceDef?.changedAlignmentType != ForceAlignmentType.None)
                {
                //Log.Message("4a");
                    changeDesc = "ForceAbilityDescChange".Translate(new object[]
                    {
                    forceDef.changedAlignmentType.ToString(),
                    Mathf.Abs(forceDef.changedAlignmentRate).ToString("0.##")
                    });
                }
                //Log.Message("5");

                if (ForceUser?.ForceSkillLevel("PJ_ForcePool") > 0)
                {
                //Log.Message("5a");
                    float poolCost = 0f;
                    //Log.Message("PC" + forceDef.forcePoolCost.ToString());
                    poolCost = forceDef.forcePoolCost - (forceDef.forcePoolCost * (0.15f * (float)ForceUser.ForceSkillLevel("PJ_ForcePool")));
                    pointsDesc = "ForceAbilityDescOriginPoints".Translate(new object[]
                    {
                    Mathf.Abs(forceDef.forcePoolCost).ToString("0.##")
                    })

                    + "\n" +

                    "ForceAbilityDescNewPoints".Translate(new object[]
                    {
                    poolCost.ToString("0.##")
                    })
                    ;
                }
                else
                {
                //Log.Message("6");

                    pointsDesc = "ForceAbilityDescPoints".Translate(new object[]
                    {
                    Mathf.Abs(forceDef.forcePoolCost).ToString("0.##")
                    });
                }
                //Log.Message("7");

                if (alignDesc != "") postDesc.AppendLine(alignDesc);
                if (changeDesc != "") postDesc.AppendLine(changeDesc);
                if (pointsDesc != "") postDesc.AppendLine(pointsDesc);
                result = postDesc.ToString();
                //Log.Message("8");

            }
            return result;
        }

        public override bool CanCastPowerCheck(AbilityContext context, out string reason)
        {
            if (base.CanCastPowerCheck(context, out reason))
            {
                reason = "";
                if (this.Def != null && this.Def is ForceAbilityDef forceDef)
                {
                    if (forceDef?.requiredAlignmentType != ForceAlignmentType.None)
                    {
                        if (forceDef?.requiredAlignmentType != ForceUtility.GetForceAlignmentType(Pawn))
                        {
                            reason = "PJ_WrongAlignment".Translate(this.Pawn.LabelShort);
                            return false;
                        }
                    }
                    if (ForceUser?.ForcePool != null)
                    {
                        if (forceDef?.forcePoolCost > 0 &&
                            ActualForceCost > ForceUtility.GetForcePool(this.Pawn)?.CurLevel)
                        {
                            reason = "PJ_DrainedForcePool".Translate(this.Pawn.LabelShort);
                            return false;
                        }
                    }
                    if (this.AbilityUser?.AbilityUser != null)
                    {
                        if (this.AbilityUser?.AbilityUser?.apparel != null)
                        {
                            if (this.AbilityUser?.AbilityUser?.apparel?.WornApparel != null && this.AbilityUser?.AbilityUser?.apparel?.WornApparelCount > 0)
                            {
                                if (this.AbilityUser?.AbilityUser?.apparel?.WornApparel?.FirstOrDefault((Apparel x) => x.def == ThingDefOf.Apparel_ShieldBelt) != null)
                                {
                                    reason = "PJ_UsingShieldBelt".Translate(this.Pawn.LabelShort);
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override bool CanOverpowerTarget(AbilityContext context, LocalTargetInfo target, out string reason)
        {
            reason = "";
            if (target.Thing is ProjectJedi.PawnGhost)
            {
                Messages.Message("PJ_ForceResisted".Translate(new object[]
                    {
                        target.Thing.LabelShort,
                        AbilityUser.AbilityUser.LabelShort,
                        this.Def.label
                        
                    }), MessageTypeDefOf.NegativeEvent);
                return false;
            }
            return base.CanOverpowerTarget(context, target, out reason);
        }
    }
}
