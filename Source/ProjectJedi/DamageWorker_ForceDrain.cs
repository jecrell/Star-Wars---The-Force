using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceDrain : DamageWorker
    {
        public override float Apply(DamageInfo dinfo, Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn != null)
            {
                if (dinfo.Instigator != null)
                {
                    Pawn caster = dinfo.Instigator as Pawn;
                    if (caster != null)
                    {
                        CompForceUser victimForce = pawn.GetComp<CompForceUser>();
                        int maxInjuries = 2;
                        int maxHeals = 0;
                        int maxPoolDamage = 30;


                        if (victimForce != null)
                        {
                            if (victimForce.IsForceUser)
                            {
                                Need_ForcePool victimForcePool = victimForce.ForcePool;
                                if (victimForcePool != null)
                                {
                                    if (victimForcePool.CurLevel > 0.1f)
                                    {
                                        //Turn 0.01f into 1, or 1.0 into 100.
                                        int victimForceInt = System.Convert.ToInt32(victimForcePool.CurLevel * 100);
                                        Log.Message("Victim Force Pool = " + victimForceInt.ToString());
                                        Need_ForcePool casterPool = caster.needs.TryGetNeed<Need_ForcePool>();
                                        if (casterPool != null)
                                        {
                                            for (int i = 0; i < Mathf.Min(victimForceInt, maxPoolDamage); i++)
                                            {
                                                if (casterPool.CurLevel >= 0.99f) break;
                                                casterPool.CurLevel += 0.01f;
                                            }
                                            return 0f;
                                        }
                                    }
                                }
                            }
                        }
                        foreach (BodyPartRecord rec in pawn.health.hediffSet.GetNotMissingParts().InRandomOrder<BodyPartRecord>())
                        {
                            if (maxInjuries > 0)
                            {
                                pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, new IntRange(5, 10).RandomInRange, -1, caster, rec));
                                maxInjuries--;
                                maxHeals++;
                            }
                        }

                        int maxInjuriesPerBodypart;
                        foreach (BodyPartRecord rec in caster.health.hediffSet.GetInjuredParts())
                        {
                            if (maxHeals > 0)
                            {
                                maxInjuriesPerBodypart = 2;
                                foreach (Hediff_Injury current in from injury in caster.health.hediffSet.GetHediffs<Hediff_Injury>() where injury.Part == rec select injury)
                                {
                                    if (maxInjuriesPerBodypart > 0)
                                    {
                                        if (current.CanHealNaturally() && !current.IsOld()) // basically check for scars and old wounds
                                        {
                                            current.Heal((int)current.Severity + 1);
                                            maxHeals--;
                                            maxInjuriesPerBodypart--;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return 0f;

        }
    }
}