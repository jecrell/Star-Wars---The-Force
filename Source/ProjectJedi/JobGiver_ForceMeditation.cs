using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using System.Collections.Generic;

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

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {

            Need_ForcePool forcePool = pawn.needs.TryGetNeed<Need_ForcePool>();

            if (forcePool == null)
            {
                return ThinkResult.NoJob;
            }

            CompForceUser compForce = pawn.TryGetComp<CompForceUser>();
            if (compForce == null)
            {
                return ThinkResult.NoJob;
            }

            Trait sensitiveTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);
            if (sensitiveTrait != null)
            {
                return ThinkResult.NoJob;
            }

            if (compForce.canMeditateTicks > Find.TickManager.TicksAbs)
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
            IntVec3 result = CellFinder.RandomClosewalkCellNearNotForbidden(pawn.Position, pawn.Map, 4, pawn);
            float closestDist = 0;
            padResult = null;
            List<Thing> meditationPads = pawn.Map.listerThings.AllThings.FindAll((Thing t) => t.Spawned && t is Building_ForceMeditationPad);
            if (meditationPads != null && meditationPads.Count > 0)
            {
                foreach (Thing pad in meditationPads)
                {
                    if (closestDist == 0)
                    {
                        result = pad.PositionHeld;
                        closestDist = pawn.PositionHeld.DistanceToSquared(pad.PositionHeld);
                        padResult = pad;
                    }
                    float newDist = pawn.PositionHeld.DistanceToSquared(pad.PositionHeld);
                    if (newDist < closestDist)
                    {
                        closestDist = newDist;
                        result = pad.PositionHeld;
                        padResult = pad;
                    }

                }
            }
            return result;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Thing padResult = null;
            IntVec3 c = ResolveMeditationLocation(pawn, out padResult);
            CompForceUser compForce = pawn.TryGetComp<CompForceUser>();
            compForce.canMeditateTicks = Find.TickManager.TicksGame + 6000;
            if (padResult != null) return new Job(DefDatabase<JobDef>.GetNamed("PJ_ForceMeditationJob"), padResult);
            else return new Job(DefDatabase<JobDef>.GetNamed("PJ_ForceMeditationJob"), c);

        }
    }
}
