using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ForceCardUtility
    {
        // RimWorld.CharacterCardUtility
        public static Vector2 ForceCardSize = new Vector2(377f, 449f);

        public static float ButtonSize = 40f;

        public static float HeaderSize = 32f;

        public static float SpacingOffset = 15f;

        public static float ColumnSize = 245f;

        public static float SkillsColumnHeight = 113f;

        public static float SkillsColumnDivider = 134f;

        public static float PowersColumnHeight = 195f;

        public static float PowersColumnWidth = 103f;

        // RimWorld.CharacterCardUtility
        public static void DrawForceCard(Rect rect, Pawn pawn)
        {
            GUI.BeginGroup(rect);


            float alignmentTextSize = Text.CalcSize("PJ_Alignment".Translate()).x;
            Rect rect2 = new Rect((rect.width / 2) - alignmentTextSize, rect.y, rect.width, HeaderSize);            
            Text.Font = GameFont.Medium;
            Widgets.Label(rect2, "PJ_Alignment".Translate().CapitalizeFirst());
            Text.Font = GameFont.Small;
            //                             Alignment

            Widgets.DrawLineHorizontal(rect.x - 10, rect2.yMax, rect.width - 15f);
            //---------------------------------------------------------------------

            
            float grayTextSize = Text.CalcSize("PJ_Gray".Translate()).x;
            float lightTextSize = Text.CalcSize("PJ_Light".Translate()).x;
            Rect rectAlignmentLabels = new Rect(0 + SpacingOffset, 0 + rect2.yMax + 2, ForceCardSize.x, ForceCardUtility.ButtonSize * 1.15f);
            Rect rectAlignmentDark = new Rect(rectAlignmentLabels.x, rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
            Rect rectAlignmentGray = new Rect((rectAlignmentLabels.x + (rectAlignmentLabels.width / 2)) - grayTextSize, rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
            Rect rectAlignmentLight = new Rect(rectAlignmentLabels.width - lightTextSize, rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
            Widgets.Label(rectAlignmentDark, "PJ_Dark".Translate().CapitalizeFirst());
            Widgets.Label(rectAlignmentGray, "PJ_Gray".Translate().CapitalizeFirst());
            Widgets.Label(rectAlignmentLight, "PJ_Light".Translate().CapitalizeFirst());

            //Dark                        Gray                        Light
            Rect rectAlignment = new Rect(rectAlignmentLabels.x, rectAlignmentLabels.yMax, rectAlignmentLabels.width, ButtonSize);

            AlignmentOnGUI(rectAlignment);
            // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

            Rect rectSkillsLabel = new Rect(rectAlignment.width / 2, rectAlignment.yMax, rectAlignment.width, HeaderSize);
            Text.Font = GameFont.Medium;
            Widgets.Label(rectSkillsLabel, "PJ_Skills".Translate().CapitalizeFirst());
            Text.Font = GameFont.Small;
            
            //                               Skills

            Widgets.DrawLineHorizontal(rect.x - 10, rectSkillsLabel.yMax, rectSkillsLabel.width - 15f);
            //---------------------------------------------------------------------

            Rect rectSkills = new Rect(rect.x, rectSkillsLabel.yMax, rectSkillsLabel.width, SkillsColumnHeight);
            Rect rectInfoPane = new Rect(rectSkills.x, rectSkills.y, SkillsColumnDivider, SkillsColumnHeight);
            Rect rectSkillsPane = new Rect(SkillsColumnDivider, rectSkills.y, rectSkills.width - SkillsColumnDivider, SkillsColumnHeight);

            InfoPane(rectInfoPane);
            SkillsPane(rectSkillsPane);

            // LEVEL ________________             |       Lightsaber Offense  [X][X][+][_][_]
            // ||||||||||||||||||||||             |       Lightsaber Defense  [+][_][_][_][_]
            // Points Available 1                 |       Lightsaber Accuracy [X][+][_][_][_]
            //

            Rect rectPowersLabel = new Rect(rectSkills.width / 2, rectSkills.yMax, rectSkills.width, HeaderSize);
            Text.Font = GameFont.Medium;
            Widgets.Label(rectPowersLabel, "PJ_Powers".Translate().CapitalizeFirst());
            Text.Font = GameFont.Small;

            //Powers

            Widgets.DrawLineHorizontal(rect.x - 10, rectPowersLabel.yMax, rectPowersLabel.width - 15f);
            //---------------------------------------------------------------------

            Rect rectPowers = new Rect(rect.x, rectPowersLabel.yMax, rectPowersLabel.width, PowersColumnHeight);
            Rect rectPowersDark = new Rect(rectPowers.x, rectPowers.y, PowersColumnWidth, PowersColumnHeight);
            Rect rectPowersGray = new Rect(rectPowers.x + PowersColumnWidth, rectPowers.y, PowersColumnWidth, PowersColumnHeight);
            Rect rectPowersLight = new Rect(rectPowers.x + PowersColumnWidth + PowersColumnWidth, rectPowers.y, PowersColumnWidth, PowersColumnHeight);

            PowersDark(rectPowersDark);
            PowersGray(rectPowersGray);
            PowersLight(rectPowersLight);

            GUI.EndGroup();
        }

        #region AlignmentGUI
        public static void AlignmentOnGUI(Rect rect)
        {
            ////base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
            //if (rect.height > 70f)
            //{
            //    float num = (rect.height - 70f) / 2f;
            //    rect.height = 70f;
            //    rect.y += num;
            //}
            ////if (Mouse.IsOver(rect))
            ////{
            ////    Widgets.DrawHighlight(rect);
            ////}
            ////TooltipHandler.TipRegion(rect, new TipSignal(() => this.GetTipString(), rect.GetHashCode()));
            //float num2 = 14f;
            //float num3 = num2 + 15f;
            //if (rect.height < 50f)
            //{
            //    num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            //}
            //Text.Font = ((rect.height <= 55f) ? GameFont.Tiny : GameFont.Small);
            //Text.Anchor = TextAnchor.LowerLeft;
            //Rect rect2 = new Rect(rect.x + num3 + rect.width * 0.1f, rect.y, rect.width - num3 - rect.width * 0.1f, rect.height / 2f);
            //Widgets.Label(rect2, this.LabelCap);
            //Text.Anchor = TextAnchor.UpperLeft;
            //Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height / 2f);
            //rect3 = new Rect(rect3.x + num3, rect3.y, rect3.width - num3 * 2f, rect3.height - num2);
            //Widgets.FillableBar(rect3, this.CurLevelPercentage, Buttons.RedTex);
            ////else Widgets.FillableBar(rect3, this.CurLevelPercentage);
            ////Widgets.FillableBarChangeArrows(rect3, this.GUIChangeArrow);
            //if (this.threshPercents != null)
            //{
            //    for (int i = 0; i < this.threshPercents.Count; i++)
            //    {
            //        this.DrawBarThreshold(rect3, this.threshPercents[i]);
            //    }
            //}
            //float curInstantLevelPercentage = this.CurInstantLevelPercentage;
            //if (curInstantLevelPercentage >= 0f)
            //{
            //    this.DrawBarInstantMarkerAt(rect3, curInstantLevelPercentage);
            //}
            //if (!this.def.tutorHighlightTag.NullOrEmpty())
            //{
            //    UIHighlighter.HighlightOpportunity(rect, this.def.tutorHighlightTag);
            //}
            //Text.Font = GameFont.Small;
        }

        private void DrawBarThreshold(Rect barRect, float threshPct)
        {
            //float num = (float)((barRect.width <= 60f) ? 1 : 2);
            //Rect position = new Rect(barRect.x + barRect.width * threshPct - (num - 1f), barRect.y + barRect.height / 2f, num, barRect.height / 2f);
            //Texture2D image;
            //if (threshPct < this.CurLevelPercentage)
            //{
            //    image = BaseContent.BlackTex;
            //    GUI.color = new Color(1f, 1f, 1f, 0.9f);
            //}
            //else
            //{
            //    image = BaseContent.GreyTex;
            //    GUI.color = new Color(1f, 1f, 1f, 0.5f);
            //}
            //GUI.DrawTexture(position, image);
            //GUI.color = Color.white;
        }
        #endregion AlignmentGUI

        #region InfoPane
        public static void InfoPane(Rect inRect)
        {

        }
        #endregion InfoPane

        #region SkillsPane
        public static void SkillsPane(Rect inRect)
        {

        }

        #endregion SkillsPane

        #region PowersDark
        public static void PowersDark(Rect inRect)
        {

        }
        #endregion PowersDark
        #region PowersGray
        public static void PowersGray(Rect inRect)
        {

        }
        #endregion PowersGray
        #region PowersLight
        public static void PowersLight(Rect inRect)
        {

        }
        #endregion PowersLight

    }
}
