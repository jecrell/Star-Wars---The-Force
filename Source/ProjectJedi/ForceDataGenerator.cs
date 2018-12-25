using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public static class ForceDataGenerator
    {
        public static ForceData RandomForceData(CompForceUser newUser)
        {
            ForceData forceData = new ForceData(newUser);

            // are his powers discovered?
            // the older the pawn more chance to have his powers discovered
            if(new IntRange(1, 101).RandomInRange > forceData?.Pawn.ageTracker.AgeBiologicalYears)
            {
                return forceData;
            }

            // generate the level
            int forceLevel = Mathf.Clamp(forceData.Pawn.skills.skills
                                                  .Select(skill => skill.Level)
                                                  .Sum() / forceData.Pawn.skills.skills.Count, 1, 50);
            forceData.Level = forceLevel;

            //select side
            forceData.Alignment = new FloatRange(0f, 1f).RandomInRange;

            while(forceLevel != 0)
            {
                bool upgradeSkills = new IntRange(0, 2).RandomInRange == 0 ? false : true;

                if (upgradeSkills)
                {
                    var skill = forceData.Skills.RandomElement();
                    if(skill?.level < 5)
                    {
                        skill.level++;
                        forceLevel--;
                    }
                }
                else
                {
                    var power = forceData.Powers.RandomElement();
                    if(power?.level < 3)
                    {
                        forceData.Pawn.GetComp<CompForceUser>()?.LevelUpPower(power);
                        forceLevel--;
                    }
                }
            }

            return forceData;
        } 
    }
}
