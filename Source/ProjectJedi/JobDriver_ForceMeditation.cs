using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace ProjectJedi
{
    public class JobDriver_ForceMeditation : JobDriver
    {
        private Rot4 faceDir;

        public override bool TryMakePreToilReservations()
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return new Toil
            {
                initAction = delegate
                {
                    this.faceDir = ((!this.job.def.faceDir.IsValid) ? Rot4.Random : this.job.def.faceDir);

                },
                tickAction = delegate
                {
                    this.pawn.rotationTracker.FaceCell(this.pawn.Position + this.faceDir.FacingCell);
                    this.pawn.GainComfortFromCellIfPossible();
                    if (this.pawn.TryGetComp<CompForceUser>() != null)
                    {
                        CompForceUser forceComp = this.pawn.GetComp<CompForceUser>();
                        if (Find.TickManager.TicksGame % 60 == 0) forceComp.ForceUserXP++;
                        Need_ForcePool poolForce = this.pawn.needs.TryGetNeed<Need_ForcePool>();
                        if (poolForce != null)
                        {
                            if (poolForce.CurLevel < 0.99f)
                            {
                                poolForce.CurLevel += 0.0005f;
                            }
                            else
                            {
                                this.EndJobWith(JobCondition.Succeeded);
                            }
                        }
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
            Scribe_Values.Look<Rot4>(ref this.faceDir, "faceDir", default(Rot4), false);
        }
    }
}
