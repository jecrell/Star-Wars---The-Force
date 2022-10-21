using RimWorld;
using Verse;
using Verse.AI;

namespace ProjectJedi
{
    public class ThinkNode_ModNeedPercentageAbove : ThinkNode_Conditional
    {
        private NeedDef need;

        private float threshold;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var ThinkNode_ModNeedPercentageAbove = (ThinkNode_ModNeedPercentageAbove) base.DeepCopy(resolve);
            ThinkNode_ModNeedPercentageAbove.need = need;
            ThinkNode_ModNeedPercentageAbove.threshold = threshold;
            return ThinkNode_ModNeedPercentageAbove;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            //Log.Message("1");
            if (need == null)
            {
                return false;
            }

            //Log.Message("2");
            if (pawn == null)
            {
                return false;
            }

            //Log.Message("3");
            if (pawn.needs == null)
            {
                return false;
            }

            //Log.Message("4");
            if (pawn.needs.TryGetNeed(need) == null)
            {
                return false;
            }

            //Log.Message("5");
            return pawn.needs.TryGetNeed(need).CurLevelPercentage > threshold;
        }
    }
}