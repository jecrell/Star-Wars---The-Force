using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectJedi
{
    public class ForceSkill : IExposable
    {
        public string label;
        public int level;

        public ForceSkill(String newLabel, int newLevel)
        {
            label = newLabel;
            level = newLevel;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue<string>(ref label, "label", "default");
            Scribe_Values.LookValue<int>(ref level, "level", 0);
        }
    }
}
