using Verse;

namespace ProjectJedi
{
    internal static class ForceUtility
    {
        public static bool IsForceUser(this Pawn p)
        {
            if (p == null)
            {
                return false;
            }

            if (p is PawnGhost)
            {
                return true;
            }

            if (p.story?.traits is not { } t)
            {
                return false;
            }

            if (t.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                t.HasTrait(ProjectJediDefOf.PJ_SithTrait) ||
                t.HasTrait(ProjectJediDefOf.PJ_GrayTrait) ||
                t.HasTrait(ProjectJediDefOf.PJ_ForceSensitive))
            {
                return true;
            }

            return false;
        }

        public static ForceAlignmentType GetForceAlignmentType(Pawn pawn)
        {
            if (pawn?.GetComp<CompForceUser>() is { } forceUser)
            {
                return forceUser.ForceAlignmentType;
            }

            return ForceAlignmentType.None;
        }

        public static Need_ForcePool GetForcePool(this Pawn pawn)
        {
            if (pawn?.GetComp<CompForceUser>() is { } forceUser)
            {
                return forceUser.ForcePool;
            }

            return null;
        }

        public static CompForceUser GetForceUser(Pawn pawn)
        {
            if (pawn?.GetComp<CompForceUser>() is { } forceUser)
            {
                return forceUser;
            }

            return null;
        }
    }
}