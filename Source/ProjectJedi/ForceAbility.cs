using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using AbilityUser;

namespace ProjectJedi
{
    public class ForceAbility : PawnAbility
    {
        public CompForceUser ForceUser => AbilityUser as CompForceUser;
        public ForceAbilityDef ForceDef => Def as ForceAbilityDef;

        public ForceAbility() : base()
        {
        }
        
        public ForceAbility(CompAbilityUser abilityUser) : base(abilityUser)
        {
            this.abilityUser = abilityUser as CompForceUser;
        }

        public ForceAbility(Pawn user, AbilityDef pdef) : base(user, pdef)
        {

        }

        private float ActualForceCost => ForceDef.forcePoolCost - (ForceDef.forcePoolCost * (0.15f * (float)ForceUser.ForceSkillLevel("PJ_ForcePool")));
        
        public override void PostAbilityAttempt()
        {
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
                if (ForceUser.ForceSkillLevel("PJ_ForcePool") > 0)
                {
                    float poolCost = 0f;
                    //Log.Message("PC" + forceDef.forcePoolCost.ToString());
                    poolCost = forceDef.forcePoolCost - (forceDef.forcePoolCost * (0.15f * (float)ForceUser.ForceSkillLevel("PJ_ForcePool")));
                    pointsDesc = "ForceAbilityDescOriginPoints".Translate(new object[]
                    {
                    forceDef.forcePoolCost.ToString("p1")
                    })

                    + "\n" +

                    "ForceAbilityDescNewPoints".Translate(new object[]
                    {
                    poolCost.ToString("p1")
                    })
                    ;
                }
                else
                {
                    pointsDesc = "ForceAbilityDescPoints".Translate(new object[]
                    {
                    forceDef.forcePoolCost.ToString("p1")
                    });
                }
                if (alignDesc != "") postDesc.AppendLine(alignDesc);
                if (changeDesc != "") postDesc.AppendLine(changeDesc);
                if (pointsDesc != "") postDesc.AppendLine(pointsDesc);
                result = postDesc.ToString();
            }
            return result;
        }

        public override bool CanCastPowerCheck(AbilityContext context, out string reason)
        {
            if (base.CanCastPowerCheck(context, out reason))
            {
                reason = "";
                if (ForceDef != null)
                {
                    if (ForceDef.requiredAlignmentType != ForceAlignmentType.None)
                    {
                        if (ForceDef.requiredAlignmentType != ForceUser.ForceAlignmentType)
                        {
                            reason = "PJ_WrongAlignment";
                            return false;
                        }
                    }
                    if (ForceUser.ForcePool != null)
                    {
                        if (ForceDef.forcePoolCost > 0 &&
                            ActualForceCost > ForceUser.ForcePool.CurLevel)
                        {
                            reason = "PJ_DrainedForcePool";
                            return false;
                        }
                    }
                    if (this.AbilityUser.AbilityUser != null)
                    {
                        if (this.AbilityUser.AbilityUser.apparel != null)
                        {
                            if (this.AbilityUser.AbilityUser.apparel.WornApparel != null && this.AbilityUser.AbilityUser.apparel.WornApparelCount > 0)
                            {
                                if (this.AbilityUser.AbilityUser.apparel.WornApparel.FirstOrDefault((Apparel x) => x.def == ThingDefOf.Apparel_ShieldBelt) != null)
                                {
                                    reason = "PJ_UsingShieldBelt";
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
                    }), MessageSound.Negative);
                return false;
            }
            return base.CanOverpowerTarget(context, target, out reason);
        }
    }
}
