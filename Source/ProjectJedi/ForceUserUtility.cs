using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectJedi
{
    static class ForceUserUtility
    {
        public static ForceAlignmentType GetForceAlignmentType(Pawn pawn)
        {
            if (pawn.GetComp<CompForceUser>() is CompForceUser forceUser)
            {
                return forceUser.ForceAlignmentType;
            }
            return ForceAlignmentType.None;
        }

        public static Need_ForcePool GetForcePool(Pawn pawn)
        {
            if (pawn.GetComp<CompForceUser>() is CompForceUser forceUser)
            {
                return forceUser.ForcePool;
            }
            return null;
        }

        public static CompForceUser GetForceUser(Pawn pawn)
        {
            if (pawn.GetComp<CompForceUser>() is CompForceUser forceUser)
            {
                return forceUser;
            }
            return null;
        }
    }
}
