using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace ProjectJedi
{
    public class DamageWorker_ForceChoke : DamageWorker
    {
        public override float Apply(DamageInfo dinfo, Thing victim)
        {
            if (victim is ProjectJedi.PawnGhost)
            {
                Messages.Message("PJ_ForceGhostResisted".Translate(), MessageSound.Negative);
                return 0f;
            }

            Pawn pawn = victim as Pawn;
            if (pawn != null)
            {
                DamageInfo newDinfo = new DamageInfo(dinfo);
                BodyPartRecord neckRecord = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.label == "neck");
                if (!pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any((Hediff_MissingPart x) => x.Part == neckRecord))
                {
                    newDinfo.SetForcedHitPart(neckRecord);
                }
                else
                {
                    BodyPartRecord lungRecord = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.tags.First((string s) => s == "BreathingSource" || s == "BreathingPathway") != null);
                    if (!pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any((Hediff_MissingPart x) => x.Part == lungRecord))
                    {
                        newDinfo.SetForcedHitPart(lungRecord);
                    }
                }
                DamageWorker_AddInjury newWorker = new DamageWorker_AddInjury();
                return newWorker.Apply(newDinfo, victim);
            }
            return 0f;
        }
        
    }
}
