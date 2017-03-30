using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ForcePower : IExposable
    {
        public string label;
        public Texture2D icon;
        public int level;

        public ForcePower(String newLabel, Texture2D newIcon, int newLevel)
        {
            label = newLabel;
            icon = newIcon;
            level = newLevel;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue<string>(ref label, "label", "default");
            Scribe_Values.LookValue<Texture2D>(ref icon, "icon", TexButton.PJTex_MindTrick);
            Scribe_Values.LookValue<int>(ref level, "level", 0);
        }
    }
}
