using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace ProjectJedi
{
    public class ThinkNode_ModNeedPercentageAbove : ThinkNode_Conditional
    {
        private NeedDef need;

        private float threshold;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ModNeedPercentageAbove ThinkNode_ModNeedPercentageAbove = (ThinkNode_ModNeedPercentageAbove)base.DeepCopy(resolve);
            ThinkNode_ModNeedPercentageAbove.need = this.need;
            ThinkNode_ModNeedPercentageAbove.threshold = this.threshold;
            return ThinkNode_ModNeedPercentageAbove;
        }

        protected override bool Satisfied(Pawn pawn)
        {
           //Log.Message("1");
            if (need == null) return false;
           //Log.Message("2");
            if (pawn == null) return false;
           //Log.Message("3");
            if (pawn.needs == null) return false;
           //Log.Message("4");
            if (pawn.needs.TryGetNeed(this.need) == null) return false;
           //Log.Message("5");
            return pawn.needs.TryGetNeed(this.need).CurLevelPercentage > this.threshold;
        }
    }
}
