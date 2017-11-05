using RimWorld;
using System.Linq;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceHeal : DamageWorker
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageResult result = DamageResult.MakeNew();
            result.totalDamageDealt = 0f;
            if (thing is ProjectJedi.PawnGhost)
            {
                Messages.Message("PJ_ForceGhostResisted".Translate(), MessageTypeDefOf.NegativeEvent);
                return result;
            }

            Pawn pawn = thing as Pawn;
            if (pawn != null)
            {
                int maxInjuries = 6;
                int maxInjuriesPerBodypart;

                foreach (BodyPartRecord rec in pawn.health.hediffSet.GetInjuredParts())
                {
                    if (maxInjuries > 0)
                    {
                        maxInjuriesPerBodypart = 2;
                        foreach (Hediff_Injury current in from injury in pawn.health.hediffSet.GetHediffs<Hediff_Injury>() where injury.Part == rec select injury)
                        {
                            if (maxInjuriesPerBodypart > 0)
                            {
                                if (current.CanHealNaturally() && !current.IsOld()) // basically check for scars and old wounds
                                {
                                    current.Heal((int)current.Severity + 1);
                                    maxInjuries--;
                                    maxInjuriesPerBodypart--;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}