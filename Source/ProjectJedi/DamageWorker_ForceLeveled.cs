using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceLeveled : DamageWorker
    {
        private Pawn caster;
        public Pawn Caster
        {
            get
            {
                return caster;
            }
            set
            {
                caster = value;
            }
        }

        public virtual void ApprenticeEffect(Thing target)
        {
           //Log.Message("Placeholder: Apprentice");
        }
        public virtual void AdeptEffect(Thing target)
        {
           //Log.Message("Placeholder: Adept");
        }
        public virtual void MasterEffect(Thing target)
        {
           //Log.Message("Placeholder: Master");
        }
        
        public override float Apply(DamageInfo dinfo, Thing victim)
        {
            if (victim is ProjectJedi.PawnGhost)
            {
                Messages.Message("PJ_ForceGhostResisted".Translate(), MessageSound.Negative);
                return 0f;
            }

            int amount = dinfo.Amount;
            caster = dinfo.Instigator as Pawn;
            switch (amount)
            {
                case 1:
                    ApprenticeEffect(victim);
                    break;
                case 2:
                    AdeptEffect(victim);
                    break;
                case 3:
                    MasterEffect(victim);
                    break;
                default:
                    Log.Error(this.def.label + " only works with damages 1, 2, or 3");
                    break;
            }
            return 0f;
        }
    }
}
