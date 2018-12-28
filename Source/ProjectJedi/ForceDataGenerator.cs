using Verse;

namespace ProjectJedi
{
    public static class ForceDataGenerator
    {
        public static ForceData ForceDataForUser(CompForceUser newUser)
        {
            ForceData forceData = new ForceData(newUser);
            
            // has the pawn begun training in the way of the force?
            if(IsPawnForceSensitive(newUser.AbilityUser))
            {
                return forceData;
            }

            // generate the level
            int forceLevel = GetPawnForceLevel(newUser.AbilityUser);
            forceData.Level = forceLevel;

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

            //select side
            forceData.Alignment = new FloatRange(0f, 1f).RandomInRange;

            return forceData;
        }
        
        public static bool IsPawnForceSensitive(Pawn pawn)
        {
            var sensitiveTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_ForceSensitive);
            if(sensitiveTrait == null)
            {
                return false;
            }

            return true;
        }

        public static int GetPawnForceLevel(Pawn pawn)
        {
            int maxLevel = 0;

            var jediTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_JediTrait);
            var sithTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_SithTrait);
            var grayTrait = pawn.story.traits.GetTrait(ProjectJediDefOf.PJ_GrayTrait);

            if(jediTrait != null)
            {
                maxLevel = jediTrait.Degree;
            }
            if(sithTrait != null)
            {
                maxLevel = sithTrait.Degree;
            }
            if(grayTrait != null)
            {
                maxLevel = grayTrait.Degree;
            }

            maxLevel *= 5;
            return new IntRange(maxLevel - 4, maxLevel).RandomInRange;
        }
    }
}
