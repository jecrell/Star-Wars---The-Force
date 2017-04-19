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
        public List<AbilityDef> abilityDefs;
        public int level;
        public AbilityDef GetAbilityDef(int index)
        {
            AbilityDef result = null;
            if (abilityDefs != null && abilityDefs.Count > 0)
            {
                result = abilityDefs[0];
                
                if (index > -1 && index < abilityDefs.Count) result = abilityDefs[index];
                else if (index >= abilityDefs.Count)
                {
                    result = abilityDefs[abilityDefs.Count - 1];
                }
            }
            return result;
        }
        public AbilityDef abilityDef
        {
            get
            {
                AbilityDef result = null;
                if (abilityDefs != null && abilityDefs.Count > 0)
                {
                    result = abilityDefs[0];

                    int index = level - 1;
                    if (index > -1 && index < abilityDefs.Count) result = abilityDefs[level - 1];
                    else if (index >= abilityDefs.Count)
                    {
                        result = abilityDefs[abilityDefs.Count - 1];
                    }
                }
                return result;
            }
        }



        public AbilityDef HasAbilityDef(AbilityDef defToFind)
        {
            return this.abilityDefs.FirstOrDefault((AbilityDef x) => x == defToFind);
        }

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

        public ForcePower(List<AbilityDef> newAbilityDefs)
        {
            level = 0;
            abilityDefs = newAbilityDefs;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue<int>(ref level, "level", 0);
            Scribe_Collections.LookList<AbilityDef>(ref abilityDefs, "abilityDefs", LookMode.Def, null);
        }
    }
}
