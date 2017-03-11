using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectJedi
{
    public class ForceAbilityDef : AbilityUser.AbilityDef
    {
        public float forcePoolCost; //p

        public bool requiresAlignment = false;

        public ForceAlignmentType requiredAlignmentType;
        //
        public bool changesAlignment = false;

        public ForceAlignmentType changedAlignmentType;
        //
        public float changedAlignmentRate;
    }
}
