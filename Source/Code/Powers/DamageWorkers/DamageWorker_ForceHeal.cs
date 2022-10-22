using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceHeal : DamageWorker
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

            var maxInjuries = 6;

            foreach (var rec in pawn.health.hediffSet.GetInjuredParts())
            {
                if (maxInjuries <= 0)
                {
                    continue;
                }

                var maxInjuriesPerBodypart = 2;
                
                List<Hediff_Injury> injuries = new List<Hediff_Injury>();
                pawn.health.hediffSet.GetHediffs<Hediff_Injury>(ref injuries, (Hediff_Injury x) => x != null && x.def.everCurableByItem && !x.IsPermanent());
                
                foreach (var current in from injury in injuries
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
                    maxInjuries--;
                    maxInjuriesPerBodypart--;
                }
            }

            return result;
        }
    }
}