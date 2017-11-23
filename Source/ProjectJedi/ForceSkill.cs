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
        public string desc;
        public int level;

        public ForceSkill()
        {

        }

        public ForceSkill(String newLabel, String newDesc)
        {
            label = newLabel;
            desc = newDesc;
            level = 0;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref label, "label", "default");
            Scribe_Values.Look<string>(ref desc, "desc", "default");
            Scribe_Values.Look<int>(ref level, "level", 0);
        }
    }
}
