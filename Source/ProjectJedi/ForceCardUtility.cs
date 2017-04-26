using Harmony;
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
        public static Vector2 ForceCardSize = new Vector2(395f, 536f);

        public static float ButtonSize = 40f;

        public static float ForceButtonSize = 46f;

        public static float ForceButtonPointSize = 24f;

        public static float HeaderSize = 32f;

        public static float TextSize = 22f;

        public static float Padding = 3f;

        public static float SpacingOffset = 15f;

        public static float SectionOffset = 8f;

        public static float ColumnSize = 245f;

        public static float SkillsColumnHeight = 113f;

        public static float SkillsColumnDivider = 114f;

        public static float SkillsTextWidth = 138f;

        public static float SkillsBoxSize = 18f;

        public static float PowersColumnHeight = 195f;

        public static float PowersColumnWidth = 123f;

        // RimWorld.CharacterCardUtility
        public static void DrawForceCard(Rect rect, Pawn pawn)
        {
            GUI.BeginGroup(rect);

            CompForceUser compForce = pawn.GetComp<CompForceUser>();
            if (compForce != null)
            {
                if (compForce.ForceUserLevel > 0)
                {
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
                    Rect rectAlignment = new Rect(rect.x, rectAlignmentLabels.yMax / 1.5f, rectAlignmentLabels.width - 20f, TextSize);

                    AlignmentOnGUI(rectAlignment, pawn.GetComp<CompForceUser>());
                    // |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

                    float skillsTextSize = Text.CalcSize("PJ_Skills".Translate()).x;
                    Rect rectSkillsLabel = new Rect((rectAlignmentLabels.width / 2) - skillsTextSize, rectAlignment.yMax + SectionOffset, rect.width, HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rectSkillsLabel, "PJ_Skills".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;

                    //                               Skills

                    Widgets.DrawLineHorizontal(rect.x - 10, rectSkillsLabel.yMax + Padding, rect.width - 15f);
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
                    Rect rectPowersLabel = new Rect((rect.width / 2) - (powersTextSize / 2), rectSkills.yMax + SectionOffset, rect.width, HeaderSize);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rectPowersLabel, "PJ_Powers".Translate().CapitalizeFirst());
                    Text.Font = GameFont.Small;

                    //Powers

                    Widgets.DrawLineHorizontal(rect.x - 10, rectPowersLabel.yMax, rect.width - 15f);
                    //---------------------------------------------------------------------

                    Rect rectPowers = new Rect(rect.x, rectPowersLabel.yMax + SectionOffset, rectPowersLabel.width, PowersColumnHeight);
                    Rect rectPowersDark = new Rect(rectPowers.x, rectPowers.y, PowersColumnWidth, PowersColumnHeight);
                    Rect rectPowersGray = new Rect(rectPowers.x + PowersColumnWidth, rectPowers.y, PowersColumnWidth, PowersColumnHeight);
                    Rect rectPowersLight = new Rect(rectPowers.x + PowersColumnWidth + PowersColumnWidth, rectPowers.y, PowersColumnWidth, PowersColumnHeight);

                    PowersGUIHandler(rectPowersDark, pawn.GetComp<CompForceUser>(), pawn.GetComp<CompForceUser>().ForcePowersDark, TexButton.PJTex_ForcePointDark);
                    PowersGUIHandler(rectPowersGray, pawn.GetComp<CompForceUser>(), pawn.GetComp<CompForceUser>().ForcePowersGray, TexButton.PJTex_ForcePointGray);
                    PowersGUIHandler(rectPowersLight, pawn.GetComp<CompForceUser>(), pawn.GetComp<CompForceUser>().ForcePowersLight, TexButton.PJTex_ForcePointLight);
                }
                else
                {
                    Rect rectInfoPane = new Rect(rect.x, rect.y, rect.width, rect.height);
                    InfoPaneSensitive(rectInfoPane, pawn.GetComp<CompForceUser>());
                }
            }

            GUI.EndGroup();
        }

        #region AlignmentGUI
        public static string AlignmentTipString(CompForceUser compForce, bool sensitive)
        {
            return "PJ_AlignmentDesc".Translate();

        }

        public static string ForceXPTipString(CompForceUser compForce, bool sensitive)
        {
            if (!sensitive) return compForce.ForceUserXP.ToString() + " / " + compForce.ForceUserXPTillNextLevel.ToString() + "\n" + "PJ_ForceXPDesc".Translate();
            return "PJ_ForceSensitiveDesc".Translate(new object[] {
                compForce.abilityUser.LabelShort
            });

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
            TooltipHandler.TipRegion(rect, new TipSignal(() => AlignmentTipString(compForce, false), rect.GetHashCode()));
            float num2 = 14f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width - 10f, rect.height);
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
            Rect rectLevel = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rectLevel, "PJ_Level".Translate().CapitalizeFirst() + " " + compForce.ForceUserLevel.ToString());
            Text.Font = GameFont.Small;

            if (DebugSettings.godMode)
            {
                Rect rectDebugPlus = new Rect(rectLevel.xMax, inRect.y, inRect.width * 0.3f, TextSize);
                if (Widgets.ButtonText(rectDebugPlus, "+", true, false, true))
                {
                    compForce.LevelUp(true);
                }
            }

            //Level 0

            Rect rectPointsAvail = new Rect(inRect.x, rectLevel.yMax, inRect.width, TextSize);
            Text.Font = GameFont.Tiny;
            Widgets.Label(rectPointsAvail, compForce.abilityPoints + " " + "PJ_PointsAvail".Translate());
            Text.Font = GameFont.Small;

            //0 points available

            Rect rectLevelBar = new Rect(rectPointsAvail.x, rectPointsAvail.yMax + 3f, inRect.width - 10f, HeaderSize * 0.6f);
            DrawLevelBar(rectLevelBar, compForce);

        }

        public static void InfoPaneSensitive(Rect inRect, CompForceUser compForce)
        {
            Rect rectLevel = new Rect(inRect.x, inRect.y, inRect.width * 0.7f, TextSize);
            Text.Font = GameFont.Small;
            Widgets.Label(rectLevel, "PJ_SensitiveMessage".Translate(new object[] {
                compForce.abilityUser.LabelShort
            }
            ));
            Text.Font = GameFont.Small;

            if (DebugSettings.godMode)
            {
                Rect rectDebugPlus = new Rect(rectLevel.xMax, inRect.y, inRect.width * 0.3f, TextSize);
                if (Widgets.ButtonText(rectDebugPlus, "+", true, false, true))
                {
                    compForce.LevelUp(true);
                }
            }
            
            //Something is awakening...

            Rect rectPointsAvail = new Rect(inRect.x, rectLevel.yMax, inRect.width, TextSize);
            Rect rectLevelBar = new Rect(rectPointsAvail.x, rectPointsAvail.yMax + 3f, inRect.width - 10f, HeaderSize * 0.6f);
            DrawLevelBar(rectLevelBar, compForce, true);

        }
        public static void DrawLevelBar(Rect rect, CompForceUser compForce, bool sensitive = false)
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
            TooltipHandler.TipRegion(rect, new TipSignal(() => ForceXPTipString(compForce, sensitive), rect.GetHashCode()));
            float num2 = 14f;
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height);
            rect3 = new Rect(rect3.x, rect3.y, rect3.width, rect3.height - num2);
            Widgets.FillableBar(rect3, compForce.XPTillNextLevelPercent, (Texture2D)AccessTools.Field(typeof(Widgets), "BarFullTexHor").GetValue(null), BaseContent.GreyTex, false);
        }
        #endregion InfoPane

        #region SkillsPane
        public static void SkillsPane(Rect inRect, CompForceUser compForce)
        {
            float currentYOffset = inRect.y;

            foreach (ForceSkill skill in compForce.ForceSkills)
            {
                Rect lightsaberOffense = new Rect(inRect.x, currentYOffset, inRect.width, TextSize);
                Rect lightsaberOffenseLabel = new Rect(inRect.x, currentYOffset, SkillsTextWidth, TextSize);
                Widgets.Label(lightsaberOffenseLabel, skill.label.Translate());

                TooltipHandler.TipRegion(lightsaberOffenseLabel, new TipSignal(() => skill.desc.Translate(), lightsaberOffenseLabel.GetHashCode()));
                Rect lightsaberOffensiveBoxes = new Rect(lightsaberOffenseLabel.xMax, currentYOffset, inRect.width - SkillsTextWidth, TextSize);

                for (int i = 1; i <= 5; i++)
                {
                    Rect lightsaberCheckbox = new Rect(lightsaberOffensiveBoxes.x + (SkillsBoxSize * i), lightsaberOffensiveBoxes.y, SkillsBoxSize, TextSize);
                    if (skill.level >= i)
                    {
                        Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxFull, 1f);
                        continue;
                    }
                    else if ((i - skill.level == 1 && compForce.abilityPoints > 0 && skill.level < 5) && (compForce.abilityUser.Faction == Faction.OfPlayer))
                    {
                        //TooltipHandler.TipRegion(rectRename, "RenameTemple".Translate());
                        if (Widgets.ButtonImage(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize - 4), TexButton.PJTex_SkillBoxAdd))
                        {
                            compForce.abilityPoints--;
                            skill.level++;
                        }
                        //Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBoxAdd, 1f);
                        continue;
                    }
                    else
                    {
                        Widgets.DrawTextureFitted(new Rect(lightsaberCheckbox.x, lightsaberCheckbox.y, lightsaberCheckbox.width - 2, TextSize), TexButton.PJTex_SkillBox, 1f);
                        continue;
                    }
                }

                currentYOffset += TextSize;
            }
        }
        #endregion SkillsPane

        #region PowersGUI
        public static void PowersGUIHandler(Rect inRect, CompForceUser compForce, List<ForcePower> forcePowers, Texture2D pointTexture)
        {
            float buttonYOffset = inRect.y;
            foreach (ForcePower power in forcePowers)
            {
                if (compForce.abilityPoints == 0 || power.level >= 3)
                {
                    Widgets.DrawTextureFitted(new Rect(inRect.x, buttonYOffset, ForceButtonSize, ForceButtonSize), power.Icon, 1.0f);
                }
                else if(Widgets.ButtonImage(new Rect(inRect.x, buttonYOffset, ForceButtonSize, ForceButtonSize), power.Icon) && (compForce.abilityUser.Faction == Faction.OfPlayer))
                {
                    ForceAbilityDef powerDef = power.nextLevelAbilityDef as ForceAbilityDef;
                    if (compForce.LightsidePoints < powerDef.lightsideTreePointsRequired)
                    {
                        Messages.Message("PJ_LightsidePointsRequired".Translate(new object[]
                        {
                            powerDef.lightsideTreePointsRequired
                        }), MessageSound.RejectInput);
                        return;
                    }
                    if (compForce.DarksidePoints < powerDef.darksideTreePointsRequired)
                    {
                        Messages.Message("PJ_DarksidePointsRequired".Translate(new object[]
                        {  
                            powerDef.darksideTreePointsRequired
                        }), MessageSound.RejectInput);
                        return;
                    }
                    if (compForce.abilityPoints < powerDef.abilityPoints)
                    {
                        Messages.Message("PJ_NotEnoughAbilityPoints".Translate(new object[]
                        {
                            compForce.abilityPoints,
                            powerDef.abilityPoints
                        }), MessageSound.RejectInput);
                        return;
                    }
                    compForce.LevelUpPower(power);
                    compForce.abilityPoints -= powerDef.abilityPoints;
                }
                for (int i = 0; i < 3; i++)
                {
                    float drawXOffset = ForceButtonSize + 1f;
                    if (i != 0) drawXOffset += (ForceButtonPointSize * i);

                    float drawYOffset = buttonYOffset + (ForceButtonSize / 3f);
                    Rect powerRegion = new Rect(inRect.x + drawXOffset, drawYOffset, ForceButtonPointSize, ForceButtonPointSize);

                    if (power.level > i)
                    {
                        Widgets.DrawTextureFitted(powerRegion, pointTexture, 1.0f);
                    }
                    else
                    {
                        Widgets.DrawTextureFitted(powerRegion, TexButton.PJTex_ForcePointDim, 1.0f);
                    }
                    ForceAbilityDef powerDef = power.GetAbilityDef(i) as ForceAbilityDef;
                    if (powerDef != null)
                    {
                        TooltipHandler.TipRegion(powerRegion, () => powerDef.GetDescription() + "\n" + compForce.PostAbilityVerbCompDesc(powerDef.MainVerb) + "\n" + powerDef.GetPointDesc() , 398462);
                    }
                
                }
                buttonYOffset += ForceButtonSize + 1;
            }
        }
        #endregion PowersGUI
    }
}
