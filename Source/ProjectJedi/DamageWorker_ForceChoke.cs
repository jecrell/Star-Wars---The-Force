using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace ProjectJedi
{
    public class DamageWorker_ForceChoke : DamageWorker_AddInjury
    {
        protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            BodyPartRecord rec = null;
            BodyPartRecord neckRecord = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.label == "neck");
            if (!pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any((Hediff_MissingPart x) => x.Part == neckRecord))
            {
                rec = neckRecord;
            }
            else
            {
                BodyPartRecord lungRecord = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.tags.First((string s) => s == "BreathingSource" || s == "BreathingPathway") != null);
                if (!pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any((Hediff_MissingPart x) => x.Part == lungRecord))
                {
                    rec = lungRecord;
                }
            }
            return rec;
        }

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, ref DamageWorker.DamageResult result)
        {
            base.FinalizeAndAddInjury(pawn, totalDamage, dinfo, ref result);
        }
    }
    //public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
    //    {
    //        DamageResult result = DamageResult.MakeNew();
    //        result.totalDamageDealt = 0f;

    //        if (victim is ProjectJedi.PawnGhost)
    //        {
    //            Messages.Message("PJ_ForceGhostResisted".Translate(), MessageTypeDefOf.NegativeEvent);
    //            return result;
    //        }

    //        if (victim is Pawn p && p.RaceProps.IsMechanoid)
    //        {
    //            Messages.Message("PJ_ForceResisted".Translate(new object[] {
    //                p.Label.CapitalizeFirst(), dinfo.Instigator.LabelShort, dinfo.Def.label
    //            }), MessageTypeDefOf.NegativeEvent);
    //            return result;
    //        }

    //        Pawn pawn = victim as Pawn;
    //        if (pawn != null)
    //        {
    //            DamageInfo newDinfo = new DamageInfo(dinfo);
    //            BodyPartRecord neckRecord = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.label == "neck");
    //            if (!pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any((Hediff_MissingPart x) => x.Part == neckRecord))
    //            {
    //                newDinfo.SetHitPart(neckRecord);
    //            }
    //            else
    //            {
    //                BodyPartRecord lungRecord = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.tags.First((string s) => s == "BreathingSource" || s == "BreathingPathway") != null);
    //                if (!pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>().Any((Hediff_MissingPart x) => x.Part == lungRecord))
    //                {
    //                    newDinfo.SetHitPart(lungRecord);
    //                }
    //            }
    //            DamageWorker_AddInjury newWorker = new DamageWorker_AddInjury();
    //            return newWorker.Apply(newDinfo, victim);
    //        }
    //        return result;
    //    }
        
    //}
}
