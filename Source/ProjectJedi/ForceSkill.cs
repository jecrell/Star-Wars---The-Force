using Verse;

namespace ProjectJedi
{
    public class ForceSkill : IExposable
    {
        public string desc;
        public string label;
        public int level;

        public ForceSkill()
        {
        }

        public ForceSkill(string newLabel, string newDesc)
        {
            label = newLabel;
            desc = newDesc;
            level = 0;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref label, "label", "default");
            Scribe_Values.Look(ref desc, "desc", "default");
            Scribe_Values.Look(ref level, "level");
        }
    }
}