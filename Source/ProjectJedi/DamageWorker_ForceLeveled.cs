using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForceLeveled : DamageWorker
    {
        public Pawn Caster { get; set; }

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

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var result = new DamageResult
            {
                totalDamageDealt = 0f
            };
            if (victim is PawnGhost)
            {
                Messages.Message("PJ_ForceGhostResisted".Translate(), MessageTypeDefOf.NegativeEvent);
                return result;
            }

            var amount = (int) dinfo.Amount;
            Caster = dinfo.Instigator as Pawn;
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
                    Log.Error(def.label + " only works with damages 1, 2, or 3");
                    break;
            }

            return result;
        }
    }
}