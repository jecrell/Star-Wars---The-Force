using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProjectJedi
{
    public class Building_ForceMeditationPad : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (var c in base.GetFloatMenuOptions(selPawn))
            {
                yield return c;
            }

            if (!selPawn.RaceProps.Humanlike || selPawn.Drafted || Faction != Faction.OfPlayer)
            {
                yield break;
            }

            var compForce = selPawn.TryGetComp<CompForceUser>();

            void meditate()
            {
                if (!selPawn.CanReserveAndReach(this, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    return;
                }

                compForce.ForceData.TicksUntilMeditate = Find.TickManager.TicksGame + 6000;
                var newJob = new Job(DefDatabase<JobDef>.GetNamed("PJ_ForceMeditationJob"), this);
                selPawn.jobs.TryTakeOrderedJob(newJob);
                selPawn.mindState.ResetLastDisturbanceTick();
            }

            if (!selPawn.CanReserve(this))
            {
                yield return new FloatMenuOption(
                    "PJ_ForceMeditate".Translate() + " (" + "Reserved".Translate() + ")", null);
            }
            else if (compForce == null || compForce.ForceUserLevel < 1)
            {
                yield return new FloatMenuOption(
                    "PJ_ForceMeditate".Translate() + " (" + "PJ_ForceMeditate_NeedForceUsersOnly".Translate() + ")",
                    null);
            }
            else if (compForce.ForceData.TicksUntilMeditate > Find.TickManager.TicksGame)
            {
                yield return new FloatMenuOption(
                    "PJ_ForceMeditate".Translate() + " (" + "PJ_ForceMeditate_NeedRest".Translate() + ")", null);
            }
            else
            {
                yield return new FloatMenuOption("PJ_ForceMeditate".Translate(), meditate);
            }

            //if (Verse.DebugSettings.godMode)
            //{
            //    yield return new FloatMenuOption("DEBUG" + "PJ_ForceMeditate".Translate(), meditate, MenuOptionPriority.Default, null, null, 0f, null, null);
            //}
        }
    }
}