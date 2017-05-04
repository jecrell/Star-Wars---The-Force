using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class PawnGhost : Pawn
    {
        public static readonly int ticksToDestroy = 1800; //30 seconds
        private int ticksLeft;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {

            ticksLeft = ticksToDestroy;
            base.SpawnSetup(map, respawningAfterLoad);
        }
        

        public override void Tick()
        {
            base.Tick();
            ticksLeft--;
            if (ticksLeft <= 0) this.Destroy();

            if (Spawned)
            {
                if (effecter == null)
                {
                    EffecterDef progressBar = EffecterDefOf.ProgressBar;
                    effecter = progressBar.Spawn();
                }
                else
                {
                    LocalTargetInfo target = this;
                    if (this.Spawned)
                    {
                        effecter.EffectTick(this, TargetInfo.Invalid);
                    }
                    MoteProgressBar mote = ((SubEffecter_ProgressBar)effecter.children[0]).mote;
                    if (mote != null)
                    {
                        float result = 1f - (float)(PawnGhost.ticksToDestroy - this.ticksLeft) / (float)PawnGhost.ticksToDestroy;

                        mote.progress = Mathf.Clamp01(result);
                        mote.offsetZ = -0.5f;
                    }
                }
            }
        }

        Effecter effecter = null;

        public override void DeSpawn()
        {
            if (effecter != null) effecter.Cleanup();
            base.DeSpawn();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0);
        }
    }
}
