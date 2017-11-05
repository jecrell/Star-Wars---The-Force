using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectJedi
{
    public class ForceData : IExposable
    {
#region Variables
        private Pawn pawn;
        private int level = 0;
        private int xp = 1;
        private int abilityPoints = 0;
        private int ticksUntilXPGain = -1;
        private int ticksUntilMeditate = 0;
        private float alignment;

        private bool initialized = false;

        private List<ForceSkill> skills;
        private List<ForcePower> powersDark;
        private List<ForcePower> powersGray;
        private List<ForcePower> powersLight;

        private Faction affiliation = null;
        private int ticksAffiliation = 0;
        #endregion

        #region Properties
        public bool Initialized { get => initialized; set => initialized = value; }

        public int Level { get => level; set => level = value; }
        public int XP { get => xp; set => xp = value; }
        public int AbilityPoints { get => abilityPoints; set => abilityPoints = value; }
        public int TicksUntilXPGain { get => ticksUntilXPGain; set => ticksUntilXPGain = value; }
        public int TicksUntilMeditate { get => ticksUntilMeditate; set => ticksUntilMeditate = value; }
        public int TicksAffiliation { get => ticksAffiliation; set => ticksAffiliation = value; }
        public float Alignment { get => alignment; set => alignment = value; }
        public Pawn Pawn => pawn;
        public Faction Affiliation { get => affiliation; set => affiliation = value; }


        public List<ForceSkill> Skills
        {
            get
            {
                if (skills == null)
                {
                    skills = new List<ForceSkill>
                    {
                        new ForceSkill("PJ_LightsaberOffense", "PJ_LightsaberOffense_Desc"),
                        new ForceSkill("PJ_LightsaberDefense", "PJ_LightsaberDefense_Desc"),
                        new ForceSkill("PJ_LightsaberAccuracy", "PJ_LightsaberAccuracy_Desc"),
                        new ForceSkill("PJ_LightsaberReflection", "PJ_LightsaberReflection_Desc"),
                        new ForceSkill("PJ_ForcePool", "PJ_ForcePool_Desc")
                    };
                }
                return skills;
            }
        }
        public List<ForcePower> PowersDark
        {
            get
            {
                if (powersDark == null)
                {
                    powersDark = new List<ForcePower>
                    {
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceRage_Apprentice, ProjectJediDefOf.PJ_ForceRage_Adept, ProjectJediDefOf.PJ_ForceRage_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceChoke_Apprentice, ProjectJediDefOf.PJ_ForceChoke_Adept, ProjectJediDefOf.PJ_ForceChoke_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceDrain_Apprentice, ProjectJediDefOf.PJ_ForceDrain_Adept, ProjectJediDefOf.PJ_ForceDrain_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceLightning_Apprentice, ProjectJediDefOf.PJ_ForceLightning_Adept, ProjectJediDefOf.PJ_ForceLightning_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceStorm_Apprentice, ProjectJediDefOf.PJ_ForceStorm_Adept, ProjectJediDefOf.PJ_ForceStorm_Master })
                    };
                }
                return powersDark;
            }
        }
        public List<ForcePower> PowersGray
        {
            get
            {
                if (powersGray == null)
                {
                    powersGray = new List<ForcePower>
                    {
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForcePush_Apprentice, ProjectJediDefOf.PJ_ForcePush_Adept, ProjectJediDefOf.PJ_ForcePush_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForcePull_Apprentice, ProjectJediDefOf.PJ_ForcePull_Adept, ProjectJediDefOf.PJ_ForcePull_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceSpeed_Apprentice, ProjectJediDefOf.PJ_ForceSpeed_Adept, ProjectJediDefOf.PJ_ForceSpeed_Master }),
                    };
                }
                return powersGray;
            }
        }
        public List<ForcePower> PowersLight
        {
            get
            {
                if (powersLight == null)
                {
                    powersLight = new List<ForcePower>
                    {
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceHealingSelf_Apprentice, ProjectJediDefOf.PJ_ForceHealingSelf_Adept, ProjectJediDefOf.PJ_ForceHealingSelf_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceHealingOther_Apprentice, ProjectJediDefOf.PJ_ForceHealingOther_Adept, ProjectJediDefOf.PJ_ForceHealingOther_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceDefense_Apprentice, ProjectJediDefOf.PJ_ForceDefense_Adept, ProjectJediDefOf.PJ_ForceDefense_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_MindTrick_Apprentice, ProjectJediDefOf.PJ_MindTrick_Adept, ProjectJediDefOf.PJ_MindTrick_Master }),
                        new ForcePower(new List<AbilityDef> { ProjectJediDefOf.PJ_ForceGhost_Apprentice, ProjectJediDefOf.PJ_ForceGhost_Adept, ProjectJediDefOf.PJ_ForceGhost_Master })
                    };
                }
                return powersLight;
            }
        }
        public IEnumerable<ForcePower> Powers => PowersDark.Concat(PowersGray.Concat(PowersLight));

        #endregion

        public ForceData()
        {

        }

        public ForceData(CompForceUser newUser)
        {
            this.pawn = newUser.AbilityUser;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn");
            Scribe_Values.Look<float>(ref this.alignment, "alignment", 0.5f);
            Scribe_Values.Look<int>(ref this.level, "level", 0);
            Scribe_Values.Look<int>(ref this.xp, "xp");
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", false);
            Scribe_Values.Look<int>(ref this.abilityPoints, "abilityPoints", 0);
            Scribe_Values.Look<int>(ref this.ticksUntilMeditate, "ticksUntilMeditate", 0);
            Scribe_Values.Look<int>(ref this.ticksUntilXPGain, "ticksUntilXPGain", -1);
            Scribe_Values.Look<int>(ref this.ticksAffiliation, "ticksAffiliation", -1);
            Scribe_Collections.Look<ForceSkill>(ref this.skills, "skills", LookMode.Deep, new object[0]);
            Scribe_Collections.Look<ForcePower>(ref this.powersDark, "powersDark", LookMode.Deep, new object[0]);
            Scribe_Collections.Look<ForcePower>(ref this.powersGray, "powersGray", LookMode.Deep, new object[0]);
            Scribe_Collections.Look<ForcePower>(ref this.powersLight, "powersLight", LookMode.Deep, new object[0]);


        }
    }
}
