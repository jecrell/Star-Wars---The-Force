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

        public static float TextSize = 22f;

        public static float Padding = 3f;

        public static float SpacingOffset = 15f;

        public static float ColumnSize = 245f;

        public static float SkillsColumnHeight = 113f;

        public static float SkillsColumnDivider = 114f;

        public static float SkillsTextWidth = 133f;

        public static float SkillsBoxSize = 18f;

        public static float PowersColumnHeight = 195f;

        public static float PowersColumnWidth = 103f;

        // RimWorld.CharacterCardUtility
        public static void DrawForceCard(Rect rect, Pawn pawn)
        {
            GUI.BeginGroup(rect);


            float alignmentTextSize = Text.CalcSize("PJ_Alignment".Translate()).x;
            Rect rect2 = new Rect(((rect.width / 2) - alignmentTextSize) + SpacingOffset, rect.y, rect.width, HeaderSize);            
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
            Rect rectAlignmentLight = new Rect(rectAlignmentLabels.width - (lightTextSize * 2), rectAlignmentLabels.y, rectAlignmentLabels.width / 3, rectAlignmentLabels.height);
            Widgets.Label(rectAlignmentDark, "PJ_Dark".Translate().CapitalizeFirst());
            Widgets.Label(rectAlignmentGray, "PJ_Gray".Translate().CapitalizeFirst());
            Widgets.Label(rectAlignmentLight, "PJ_Light".Translate().CapitalizeFirst());

            //Dark                        Gray                        Light
            Rect rectAlignment = new Rect(rect.x, rectAlignmentLabels.yMax / 1.5f, rectAlignmentLabels.width, TextSize);

            AlignmentOnGUI(rectAlignment, pawn.GetComp<CompForceUser>());
            // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

            float skillsTextSize = Text.CalcSize("PJ_Skills".Translate()).x;
            Rect rectSkillsLabel = new Rect((rectAlignment.width / 2) - skillsTextSize, rectAlignment.yMax + 3, rectAlignment.width, HeaderSize);
            Text.Font = GameFont.Medium;
            Widgets.Label(rectSkillsLabel, "PJ_Skills".Translate().CapitalizeFirst());
            Text.Font = GameFont.Small;
            
            //                               Skills

            Widgets.DrawLineHorizontal(rect.x - 10, rectSkillsLabel.yMax + Padding, rectSkillsLabel.width - 15f);
            //---------------------------------------------------------------------

            Rect rectSkills = new Rect(rect.x, rectSkillsLabel.yMax + Padding, rectSkillsLabel.width, SkillsColumnHeight);
            Rect rectInfoPane = new Rect(rectSkills.x, rectSkills.y + Padding, SkillsColumnDivider, SkillsColumnHeight);
            Rect rectSkillsPane = new Rect(rectSkills.x + SkillsColumnDivider, rectSkills.y + Padding, rectSkills.width - SkillsColumnDivider, SkillsColumnHeight);

            InfoPane(rectInfoPane, pawn.GetComp<CompForceUser>());
            SkillsPane(rectSkillsPane, pawn.GetComp<CompForceUser>());

            // LEVEL ________________             |       Lightsaber Offense  [X][X][+][_][_]
            // ||||||||||||||||||||||             |       Lightsaber Defense  [+][_][_][_][_]
            // Points Available 1                 |       Lightsaber Accuracy [X][+][_][_][_]
            //

            float powersTextSize = Text.CalcSize("PJ_Powers".Translate()).x;
            Rect rectPowersLabel = new Rect((rectSkills.width / 2) - powersTextSize, rectSkills.yMax, rectSkills.width, HeaderSize);
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
        public static string AlignmentTipString()
        {
            return "";
        }

        public static void AlignmentOnGUI(Rect rect, CompForceUser compForce)
        {
            ////base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
            if (rect.height > 70f)
            {
                float num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            TooltipHandler.TipRegion(rect, new TipSignal(() => AlignmentTipString(), rect.GetHashCode()));
            float num2 = 14f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height);
            rect3 = new Rect(rect3.x, rect3.y, rect3.width, rect3.height - num2);
            Widgets.FillableBar(rect3, 1.0f, TexButton.PJTex_AlignmentBar);
            float curInstantLevelPercentage = compForce.AlignmentValue;
            if (curInstantLevelPercentage >= 0f)
            {
                DrawBarInstantMarkerAt(rect3, curInstantLevelPercentage);
            }
        }

        public static void DrawBarInstantMarkerAt(Rect barRect, float pct)
        {
            float num = 12f;
            if (barRect.width < 150f)
            {
                num /= 2f;
            }
            Vector2 vector = new Vector2(barRect.x + barRect.width * pct, barRect.y + barRect.height);
            Rect position = new Rect(vector.x - num / 2f, vector.y, num, num);
            GUI.DrawTexture(position, TexButton.PJTex_AlignmentBarMarker);
        }
        #endregion AlignmentGUI

        #region InfoPane
        public static void InfoPane(Rect inRect, CompForceUser compForce)
        {
            Rect rectLevel = new Rect(inRect.x, inRect.y, inRect.width, TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rectLevel, "PJ_Level".Translate().CapitalizeFirst() + " " + compForce.ForceUserLevel.ToString());
            Text.Font = GameFont.Small;

            //Level 0

            Rect rectPointsAvail = new Rect(inRect.x, rectLevel.yMax, inRect.width, TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rectPointsAvail, compForce.abilityPoints + " " + "PJ_PointsAvail".Translate());
            Text.Font = GameFont.Small;

            //0 points available

            Rect rectLevelBar = new Rect(rectPointsAvail.x, rectPointsAvail.yMax, inRect.width, HeaderSize);
            DrawLevelBar(rectLevelBar, compForce);

        }
        public static void DrawLevelBar(Rect rect, CompForceUser compForce)
        {
            ////base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
            if (rect.height > 70f)
            {
                float num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            TooltipHandler.TipRegion(rect, new TipSignal(() => AlignmentTipString(), rect.GetHashCode()));
            float num2 = 14f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height);
            rect3 = new Rect(rect3.x, rect3.y, rect3.width, rect3.height - num2);
            Widgets.FillableBar(rect3, compForce.XPTillNextLevelPercent);
        }
        #endregion InfoPane

        #region SkillsPane
        public static void SkillsPane(Rect inRect, CompForceUser compForce)
        {
            float currentYOffset = inRect.y;

            //Lightsaber Offense
            Rect lightsaberOffense = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
            Rect lightsaberOffenseLabel = new Rect(inRect.x, currentYOffset, SkillsTextWidth, TextSize);
            Widgets.Label(lightsaberOffenseLabel, "PJ_LightsaberOffense".Translate());
            Rect lightsaberOffensiveBoxes = new Rect(lightsaberOffenseLabel.xMax, currentYOffset, inRect.width - SkillsTextWidth, TextSize);

            for (int i = 0; i < 5; i++)
            {
                Rect lightsaberCheckbox = new Rect(lightsaberOffensiveBoxes.x + (SkillsBoxSize * i), lightsaberOffensiveBoxes.y, SkillsBoxSize, TextSize);
                if (compForce.levelLightsaberOff >= i)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                    continue;
                }
                else if (i - compForce.levelLightsaberOff == 1)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                    continue;
                }
                else
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                    continue;
                }
            }

            currentYOffset += TextSize;

            //Lightsaber Defense
            Rect lightsaberDefense = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
            Rect lightsaberDefenseLabel = new Rect(inRect.x, lightsaberDefense.y, SkillsTextWidth, TextSize);
            Widgets.Label(lightsaberDefenseLabel, "PJ_LightsaberDefense".Translate());
            Rect lightsaberDefenseBoxes = new Rect(lightsaberDefenseLabel.xMax, lightsaberDefense.y, inRect.width - SkillsTextWidth, TextSize);

            for (int i = 0; i < 5; i++)
            {
                Rect lightsaberCheckbox = new Rect(lightsaberDefenseBoxes.x + (SkillsBoxSize * i), lightsaberDefenseBoxes.y, SkillsBoxSize, TextSize);
                if (compForce.levelLightsaberDef >= i)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                    continue;
                }
                else if (i - compForce.levelLightsaberDef == 1)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                    continue;
                }
                else
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                    continue;
                }
            }

            currentYOffset += TextSize;

            //Lightsaber Accuracy
            Rect lightsaberAccuracy = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
            Rect lightsaberAccuracyLabel = new Rect(inRect.x, currentYOffset, SkillsTextWidth, TextSize);
            Widgets.Label(lightsaberAccuracyLabel, "PJ_LightsaberAccuracy".Translate());
            Rect lightsaberAccuracyBoxes = new Rect(lightsaberAccuracyLabel.xMax, currentYOffset, inRect.width - SkillsTextWidth, TextSize);

            for (int i = 0; i < 5; i++)
            {
                Rect lightsaberCheckbox = new Rect(lightsaberAccuracyBoxes.x + (SkillsBoxSize * i), lightsaberAccuracyBoxes.y, SkillsBoxSize, TextSize);
                if (compForce.levelLightsaberAcc >= i)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                    continue;
                }
                else if (i - compForce.levelLightsaberAcc == 1)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                    continue;
                }
                else
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                    continue;
                }
            }
            currentYOffset += TextSize;

            //Lightsaber Reflection
            Rect lightsaberReflect = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
            Rect lightsaberReflectLabel = new Rect(inRect.x, currentYOffset, SkillsTextWidth, TextSize);
            Widgets.Label(lightsaberReflectLabel, "PJ_LightsaberReflection".Translate());
            Rect lightsaberReflectBoxes = new Rect(lightsaberReflectLabel.xMax, currentYOffset, inRect.width - SkillsTextWidth, TextSize);

            for (int i = 0; i < 5; i++)
            {
                Rect lightsaberCheckbox = new Rect(lightsaberReflectBoxes.x + (SkillsBoxSize * i), lightsaberReflectBoxes.y, SkillsBoxSize, TextSize);
                if (compForce.levelLightsaberRef >= i)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                    continue;
                }
                else if (i - compForce.levelLightsaberRef == 1)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                    continue;
                }
                else
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                    continue;
                }
            }
            currentYOffset += TextSize;

            //Force Pool
            Rect forcePool = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
            Rect forcePoolLabel = new Rect(inRect.x, currentYOffset, SkillsTextWidth, TextSize);
            Widgets.Label(forcePoolLabel, "PJ_ForcePool".Translate());
            Rect forcePoolBoxes = new Rect(forcePoolLabel.xMax, currentYOffset, inRect.width - SkillsTextWidth, TextSize);

            for (int i = 0; i < 5; i++)
            {
                Rect lightsaberCheckbox = new Rect(forcePoolBoxes.x + (SkillsBoxSize * i), forcePoolBoxes.y, SkillsBoxSize, TextSize);
                if (compForce.levelForcePool >= i)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                    continue;
                }
                else if (i - compForce.levelForcePool == 1)
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                    continue;
                }
                else
                {
                    Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                    continue;
                }
            }
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
