using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectJedi
{
    [DefOf]
    public static class ProjectJediDefOf
    {
        public static TraitDef PJ_ForceSensitive;
        public static TraitDef PJ_JediTrait;
        public static TraitDef PJ_SithTrait;

        public static NeedDef PJ_ForcePool;

        public static HediffDef PJ_ForceWielderHD;

        //Light Side
        public static AbilityDef PJ_ForceHealingSelf;
        public static AbilityDef PJ_ForceHealingOther;

        //Dark Side
        public static AbilityDef PJ_ForceDrain;
        public static AbilityDef PJ_ForceLightning;
        public static AbilityDef PJ_ForceStorm;
    }
}
