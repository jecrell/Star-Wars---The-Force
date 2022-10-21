using Verse;
using Verse.AI;

namespace ProjectJedi
{
    public class JobGiver_ForceMeditation : ThinkNode_JobGiver
    {
        private ForcePoolCategory minCategory = ForcePoolCategory.Steady;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiver_ForceMeditation = (JobGiver_ForceMeditation) base.DeepCopy(resolve);
            jobGiver_ForceMeditation.minCategory = minCategory;
            return jobGiver_ForceMeditation;
        }

        public override float GetPriority(Pawn pawn)
        {
            //Log.Message("Priority");
            var forcePool = pawn.needs.TryGetNeed<Need_ForcePool>();
            if (forcePool == null)
            {
                return 0f;
            }

            var compForce = pawn.TryGetComp<CompForceUser>();
            if (compForce.ForceData.TicksUntilMeditate > Find.TickManager.TicksAbs)
            {
                return 0f;
            }

            if (forcePool.CurCategory > ForcePoolCategory.Steady)
            {
                return 0f;
            }

            if (forcePool.CurLevelPercentage < 0.5)
            {
                return 9.5f;
            }

            return 0f;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            var forcePool = pawn.needs.TryGetNeed<Need_ForcePool>();

            if (forcePool == null)
            {
                return ThinkResult.NoJob;
            }

            var compForce = pawn.TryGetComp<CompForceUser>();
            if (compForce == null)
            {
                return ThinkResult.NoJob;
            }

            var sensitiveTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);
            if (sensitiveTrait != null)
            {
                return ThinkResult.NoJob;
            }

            if (compForce.ForceData.TicksUntilMeditate > Find.TickManager.TicksAbs)
            {
                return ThinkResult.NoJob;
            }

            if (forcePool.CurCategory > ForcePoolCategory.Strong)
            {
                return ThinkResult.NoJob;
            }

            return base.TryIssueJobPackage(pawn, jobParams);
        }

        public static IntVec3 ResolveMeditationLocation(Pawn pawn, out Thing padResult)
        {
            var result = CellFinder.RandomClosewalkCellNearNotForbidden(pawn, 4);
            float closestDist = 0;
            padResult = null;
            var meditationPads =
                pawn.Map.listerThings.AllThings.FindAll(t => t.Spawned && t is Building_ForceMeditationPad);
            if (meditationPads.Count <= 0)
            {
                return result;
            }

            foreach (var pad in meditationPads)
            {
                if (closestDist == 0)
                {
                    result = pad.PositionHeld;
                    closestDist = pawn.PositionHeld.DistanceToSquared(pad.PositionHeld);
                    padResult = pad;
                }

                float newDist = pawn.PositionHeld.DistanceToSquared(pad.PositionHeld);
                if (!(newDist < closestDist))
                {
                    continue;
                }

                closestDist = newDist;
                result = pad.PositionHeld;
                padResult = pad;
            }

            return result;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            var c = ResolveMeditationLocation(pawn, out var padResult);
            var compForce = pawn.TryGetComp<CompForceUser>();
            compForce.ForceData.TicksUntilMeditate = Find.TickManager.TicksGame + 6000;
            if (padResult != null)
            {
                return new Job(DefDatabase<JobDef>.GetNamed("PJ_ForceMeditationJob"), padResult);
            }

            return new Job(DefDatabase<JobDef>.GetNamed("PJ_ForceMeditationJob"), c);
        }
    }
}