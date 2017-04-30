using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace ProjectJedi
{
    public class JobGiver_ForceMeditation : ThinkNode_JobGiver
    {
        private ForcePoolCategory minCategory = ForcePoolCategory.Steady;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_ForceMeditation jobGiver_ForceMeditation = (JobGiver_ForceMeditation)base.DeepCopy(resolve);
            jobGiver_ForceMeditation.minCategory = this.minCategory;
            return jobGiver_ForceMeditation;
        }

        public override float GetPriority(Pawn pawn)
        {
           //Log.Message("Priority");
            Need_ForcePool forcePool = pawn.needs.TryGetNeed<Need_ForcePool>();
            if (forcePool == null)
            {
                return 0f;
            }
            CompForceUser compForce = pawn.TryGetComp<CompForceUser>();
            if (compForce.canMeditateTicks > Find.TickManager.TicksAbs)
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

        public override ThinkResult TryIssueJobPackage(Pawn pawn)
        {


           //Log.Message("TryIssueJobPackage");
            Need_ForcePool forcePool = pawn.needs.TryGetNeed<Need_ForcePool>();
           //Log.Message("T1");
            if (forcePool == null)
            {
                return ThinkResult.NoJob;
            }
           //Log.Message("T2");

            CompForceUser compForce = pawn.TryGetComp<CompForceUser>();
            if (compForce == null)
            {
                return ThinkResult.NoJob;
            }
            //Log.Message("T3");


            Trait sensitiveTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);
            if (sensitiveTrait != null)
            {
                return ThinkResult.NoJob;
            }

            if (compForce.canMeditateTicks > Find.TickManager.TicksAbs)
            {
                return ThinkResult.NoJob;
            }
           //Log.Message("T4");

            if (forcePool.CurCategory > ForcePoolCategory.Strong)
            {
                return ThinkResult.NoJob;
            }
           //Log.Message("T5");

            return base.TryIssueJobPackage(pawn);
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
           //Log.Message("JobGiven");
            //if (RestUtility.DisturbancePreventsLyingDown(pawn))
            //{
            //    return null;
            //}
            IntVec3 c = CellFinder.RandomClosewalkCellNearNotForbidden(pawn.Position, pawn.Map, 4, pawn);
            CompForceUser compForce = pawn.TryGetComp<CompForceUser>();
            compForce.canMeditateTicks = Find.TickManager.TicksGame + 6000;
            return new Job(DefDatabase<JobDef>.GetNamed("PJ_ForceMeditationJob"), c);

        }
    }
}
