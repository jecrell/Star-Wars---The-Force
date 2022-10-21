using System.Collections.Generic;
using System.Linq;
using AbilityUser;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ForcePower : IExposable
    {
        public List<AbilityDef> abilityDefs;
        public int level;
        public int ticksUntilNextCast = -1;

        public ForcePower()
        {
        }

        public ForcePower(List<AbilityDef> newAbilityDefs)
        {
            level = 0;
            abilityDefs = newAbilityDefs;
        }

        public AbilityDef AbilityDef
        {
            get
            {
                if (abilityDefs == null || abilityDefs.Count <= 0)
                {
                    return null;
                }

                var result = abilityDefs[0];

                var index = level - 1;
                if (index > -1 && index < abilityDefs.Count)
                {
                    result = abilityDefs[index];
                }
                else if (index >= abilityDefs.Count)
                {
                    result = abilityDefs[abilityDefs.Count - 1];
                }

                return result;
            }
        }

        public AbilityDef NextLevelAbilityDef
        {
            get
            {
                if (abilityDefs == null || abilityDefs.Count <= 0)
                {
                    return null;
                }

                var result = abilityDefs[0];

                var index = level;
                if (index > -1 && index <= abilityDefs.Count)
                {
                    result = abilityDefs[index];
                }
                else if (index >= abilityDefs.Count)
                {
                    result = abilityDefs[abilityDefs.Count - 1];
                }

                return result;
            }
        }

        public Texture2D Icon => AbilityDef.uiIcon;


        public void ExposeData()
        {
            Scribe_Values.Look(ref level, "level");
            Scribe_Values.Look(ref ticksUntilNextCast, "ticksUntilNextCast", -1);
            Scribe_Collections.Look(ref abilityDefs, "abilityDefs", LookMode.Def, null);
        }

        public AbilityDef GetAbilityDef(int index)
        {
            if (abilityDefs == null || abilityDefs.Count <= 0)
            {
                return null;
            }

            var result = abilityDefs[0];

            if (index > -1 && index < abilityDefs.Count)
            {
                result = abilityDefs[index];
            }
            else if (index >= abilityDefs.Count)
            {
                result = abilityDefs[abilityDefs.Count - 1];
            }

            return result;
        }


        public AbilityDef HasAbilityDef(AbilityDef defToFind)
        {
            return abilityDefs.FirstOrDefault(x => x == defToFind);
        }
    }
}