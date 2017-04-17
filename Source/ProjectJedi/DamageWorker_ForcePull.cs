using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForcePull : DamageWorker_ForceLeveled
    {
        public override void ApprenticeEffect(Thing target)
        {
            base.ApprenticeEffect(target);
        }
        public override void AdeptEffect(Thing target)
        {
            base.AdeptEffect(target);
        }
        public override void MasterEffect(Thing target)
        {
            base.MasterEffect(target);
        }
    }
}
