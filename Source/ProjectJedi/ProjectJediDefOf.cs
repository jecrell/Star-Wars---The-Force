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
        public static TraitDef PJ_GrayTrait;

        public static NeedDef PJ_ForcePool;

        public static HediffDef PJ_ForceWielderHD;

        //Light Side
        public static AbilityDef PJ_ForceDefense;
        public static AbilityDef PJ_MindTrick;
        public static AbilityDef PJ_ForceHealingSelf;
        public static AbilityDef PJ_ForceHealingOther;
        public static AbilityDef PJ_ForceGhost;

        //Gray Powers
        public static AbilityDef PJ_ForcePush;
        public static AbilityDef PJ_ForcePull;
        public static AbilityDef PJ_ForceSpeed;


        //Dark Side
        public static AbilityDef PJ_ForceRage;
        public static AbilityDef PJ_ForceDrain;
        public static AbilityDef PJ_ForceChoke;
        public static AbilityDef PJ_ForceLightning;
        public static AbilityDef PJ_ForceStorm;
    }
}
