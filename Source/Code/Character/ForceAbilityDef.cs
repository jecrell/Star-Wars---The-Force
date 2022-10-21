using System.Text;
using AbilityUser;
using Verse;

namespace ProjectJedi
{
    public class ForceAbilityDef : AbilityDef
    {
        public int abilityPoints = 1;

        public float changedAlignmentRate = 0.0f;

        public ForceAlignmentType changedAlignmentType = ForceAlignmentType.None;

        public int darksideTreePointsRequired = 0;
        public float forcePoolCost = 0.0f;

        public int lightsideTreePointsRequired = 0;

        public ForceAlignmentType requiredAlignmentType = ForceAlignmentType.None;


        public string GetPointDesc()
        {
            var s = new StringBuilder();
            s.AppendLine("PJ_PointsRequired".Translate(abilityPoints));
            if (darksideTreePointsRequired > 0)
            {
                s.AppendLine("PJ_DarkPointsRequired".Translate(darksideTreePointsRequired));
            }

            if (lightsideTreePointsRequired > 0)
            {
                s.AppendLine("PJ_LightPointsRequired".Translate(lightsideTreePointsRequired));
            }

            var result = s.ToString();
            return result;
        }
    }
}