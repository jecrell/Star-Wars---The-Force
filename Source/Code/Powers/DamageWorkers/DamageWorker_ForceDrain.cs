using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceDrain : DamageWorker
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            var result = new DamageResult
            {
                totalDamageDealt = 0f
            };
            if (thing is PawnGhost)
            {
                Messages.Message("PJ_ForceGhostResisted".Translate(), MessageTypeDefOf.NegativeEvent);
                return result;
            }

            if (thing is not Pawn pawn)
            {
                return result;
            }

            if (dinfo.Instigator == null)
            {
                return result;
            }

            if (dinfo.Instigator is not Pawn caster)
            {
                return result;
            }

            var victimForce = pawn.GetComp<CompForceUser>();
            var maxInjuries = 2;
            var maxHeals = 0;
            var maxPoolDamage = 30;


            if (victimForce is {IsForceUser: true})
            {
                var victimForcePool = victimForce.ForcePool;
                if (victimForcePool is {CurLevel: > 0.1f})
                {
                    //Turn 0.01f into 1, or 1.0 into 100.
                    var victimForceInt = Convert.ToInt32(victimForcePool.CurLevel * 100);
                    //Log.Message("Victim Force Pool = " + victimForceInt.ToString());
                    var casterPool = caster.needs.TryGetNeed<Need_ForcePool>();
                    if (casterPool != null)
                    {
                        Messages.Message("PJ_ForceDrainOne".Translate(caster.Label, pawn.Label),
                            MessageTypeDefOf.SilentInput);
                        for (var i = 0; i < Mathf.Min(victimForceInt, maxPoolDamage); i++)
                        {
                            if (casterPool.CurLevel >= 0.99f)
                            {
                                break;
                            }

                            casterPool.CurLevel += 0.01f;
                            victimForcePool.CurLevel -= 0.05f;
                        }

                        return result;
                    }
                }
            }

            Messages.Message("PJ_ForceDrainTwo".Translate(caster.Label, pawn.Label), MessageTypeDefOf.SilentInput);

            foreach (var rec in pawn.health.hediffSet.GetNotMissingParts().InRandomOrder())
            {
                if (maxInjuries <= 0)
                {
                    continue;
                }

                pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, new IntRange(5, 10).RandomInRange, 1f,
                    -1, caster, rec));
                maxInjuries--;
                maxHeals++;
            }

            foreach (var rec in caster.health.hediffSet.GetInjuredParts())
            {
                if (maxHeals <= 0)
                {
                    continue;
                }
                
                List<Hediff_Injury> injuries = new List<Hediff_Injury>();
                caster.health.hediffSet.GetHediffs<Hediff_Injury>(ref injuries, (Hediff_Injury x) => x != null && x.def.everCurableByItem && !x.IsPermanent());

                var maxInjuriesPerBodypart = 2;
                foreach (var current in from injury in
                             injuries
                    where injury.Part == rec
                    select injury)
                {
                    if (maxInjuriesPerBodypart <= 0)
                    {
                        continue;
                    }

                    if (!current.CanHealNaturally() || current.IsPermanent())
                    {
                        continue;
                    }

                    current.Heal((int) current.Severity + 1);
                    maxHeals--;
                    maxInjuriesPerBodypart--;
                }
            }

            return result;
        }
    }
}