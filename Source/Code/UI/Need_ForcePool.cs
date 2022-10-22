using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class Need_ForcePool : Need
    {
        public const float BaseGainPerTickRate = 150f;
        public const float BaseFallPerTick = 1E-05f;
        public const float ThreshVeryLow = 0.1f;
        public const float ThreshLow = 0.3f;
        public const float ThreshSatisfied = 0.5f;
        public const float ThreshHigh = 0.7f;
        public const float ThreshVeryHigh = 0.9f;
        private int lastGainTick;

        public int ticksUntilBaseSet = 500;

        public Need_ForcePool(Pawn pawn) : base(pawn)
        {
            lastGainTick = -999;
            threshPercents = new List<float>
            {
                ThreshLow,
                ThreshHigh
            };
        }

        public ForcePoolCategory CurCategory
        {
            get
            {
                if (CurLevel < 0.01f)
                {
                    return ForcePoolCategory.Drained;
                }

                if (CurLevel < 0.3f)
                {
                    return ForcePoolCategory.Feeble;
                }

                if (CurLevel < 0.5f)
                {
                    return ForcePoolCategory.Steady;
                }

                if (CurLevel < 0.7f)
                {
                    return ForcePoolCategory.Strong;
                }

                return ForcePoolCategory.Surging;
            }
        }

        public override int GUIChangeArrow => GainingNeed ? 1 : -1;

        public override float CurInstantLevel => CurLevel;

        private bool GainingNeed => Find.TickManager.TicksGame < lastGainTick + 10;

        public override void SetInitialLevel()
        {
            CurLevel = ThreshSatisfied;
        }

        public void GainNeed(float amount)
        {
            amount /= 120f;
            amount *= 0.01f;
            amount = Mathf.Min(amount, 1f - CurLevel);
            curLevelInt += amount;
            lastGainTick = Find.TickManager.TicksGame;
        }

        public void UseForcePower(float amount)
        {
            //Log.Message(this.curLevelInt.ToString());
            //Log.Message(amount.ToString());
            curLevelInt = Mathf.Clamp(curLevelInt - amount, 0f, 1.0f);
            //Log.Message(this.curLevelInt.ToString());
        }

        public override void NeedInterval()
        {
            GainNeed(1f);
        }

        public override string GetTipString()
        {
            return base.GetTipString();
        }


        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1, bool drawArrows = true,
            bool doTooltip = true, Rect? rectForTooltip = null, bool drawLabel = true)
        {
            //base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
            if (rect.height > 70f)
            {
                var num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }

            TooltipHandler.TipRegion(rect, new TipSignal(GetTipString, rect.GetHashCode()));
            var num2 = 14f;
            var num3 = num2 + 15f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }

            Text.Font = rect.height <= 55f ? GameFont.Tiny : GameFont.Small;
            Text.Anchor = TextAnchor.LowerLeft;
            var rect2 = new Rect(rect.x + num3 + (rect.width * 0.1f), rect.y, rect.width - num3 - (rect.width * 0.1f),
                rect.height / 2f);
            Widgets.Label(rect2, LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            var rect3 = new Rect(rect.x, rect.y + (rect.height / 2f), rect.width, rect.height / 2f);
            rect3 = new Rect(rect3.x + num3, rect3.y, rect3.width - (num3 * 2f), rect3.height - num2);
            Widgets.FillableBar(rect3, CurLevelPercentage);
            //else Widgets.FillableBar(rect3, this.CurLevelPercentage);
            //Widgets.FillableBarChangeArrows(rect3, this.GUIChangeArrow);
            if (threshPercents != null)
            {
                foreach (var threshPct in threshPercents)
                {
                    DrawBarThreshold(rect3, threshPct);
                }
            }

            var curInstantLevelPercentage = CurInstantLevelPercentage;
            if (curInstantLevelPercentage >= 0f)
            {
                DrawBarInstantMarkerAt(rect3, curInstantLevelPercentage);
            }

            if (!def.tutorHighlightTag.NullOrEmpty())
            {
                UIHighlighter.HighlightOpportunity(rect, def.tutorHighlightTag);
            }

            Text.Font = GameFont.Small;
        }

        private new void DrawBarThreshold(Rect barRect, float threshPct)
        {
            float num = barRect.width <= 60f ? 1 : 2;
            var position = new Rect(barRect.x + (barRect.width * threshPct) - (num - 1f),
                barRect.y + (barRect.height / 2f), num, barRect.height / 2f);
            Texture2D image;
            if (threshPct < CurLevelPercentage)
            {
                image = BaseContent.BlackTex;
                GUI.color = new Color(1f, 1f, 1f, 0.9f);
            }
            else
            {
                image = BaseContent.GreyTex;
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }

            GUI.DrawTexture(position, image);
            GUI.color = Color.white;
        }
    }
}