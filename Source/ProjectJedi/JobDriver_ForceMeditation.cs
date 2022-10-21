using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProjectJedi
{
    public class JobDriver_ForceMeditation : JobDriver
    {
        private Rot4 faceDir;

        public override bool TryMakePreToilReservations(bool somethin)
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return new Toil
            {
                initAction = delegate { faceDir = !job.def.faceDir.IsValid ? Rot4.Random : job.def.faceDir; },
                tickAction = delegate
                {
                    pawn.rotationTracker.FaceCell(pawn.Position + faceDir.FacingCell);
                    pawn.GainComfortFromCellIfPossible();
                    if (pawn.TryGetComp<CompForceUser>() == null)
                    {
                        return;
                    }

                    var forceComp = pawn.GetComp<CompForceUser>();
                    if (Find.TickManager.TicksGame % 60 == 0)
                    {
                        forceComp.ForceUserXP++;
                    }

                    var poolForce = pawn.needs.TryGetNeed<Need_ForcePool>();
                    if (poolForce == null)
                    {
                        return;
                    }

                    if (poolForce.CurLevel < 0.99f)
                    {
                        poolForce.CurLevel += 0.0005f;
                    }
                    else
                    {
                        EndJobWith(JobCondition.Succeeded);
                    }
                    //JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 1f);
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 1800
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref faceDir, "faceDir");
        }
    }
}