using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using AbilityUser;

namespace ProjectJedi
{
    public class ForcePower : IExposable
    {
        public string label;
        public int level;
        public AbilityDef abilityDef;

        public ForcePower()
        {

        }

        public Texture2D Icon
        {
            get
            {
                return abilityDef.uiIcon;
            }
        }

        public ForcePower(String newLabel, int newLevel, AbilityDef newAbilityDef)
        {
            label = newLabel;
            level = newLevel;
            abilityDef = newAbilityDef;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue<string>(ref label, "label", "default");

            //Scribe_Deep.LookDeep<Texture2D>(ref this.icon, "icon", null);
            //Scribe_References.LookReference<Texture2D>(ref this.icon, "icon");
            //Scribe_Values.LookValue<Texture2D>(ref icon, "icon", TexButton.PJTex_MindTrick);
            Scribe_Values.LookValue<int>(ref level, "level", 0);
            Scribe_Defs.LookDef<AbilityDef>(ref this.abilityDef, "abilityDef");
        }
    }
}
