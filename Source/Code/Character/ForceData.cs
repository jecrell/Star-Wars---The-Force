using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using AbilityDef = AbilityUser.AbilityDef;

namespace ProjectJedi
{
    public class ForceData : IExposable
    {
        private readonly bool initialized = false;
        private int abilityPoints;

        private float alignment;
        public bool forcePowersInitialized;
        private int level;
        private Pawn pawn;
        private List<ForcePower> powersDark;
        private List<ForcePower> powersGray;
        private List<ForcePower> powersLight;

        private List<ForceSkill> skills;

        public bool TabResolved = false;
        private int ticksAffiliation;
        private int ticksUntilMeditate;
        private int ticksUntilXPGain = -1;
        private int xp = 1;

        public ForceData()
        {
        }

        public ForceData(CompForceUser newUser)
        {
            pawn = newUser.Pawn;
        }

        public bool ForcePowersInitialized
        {
            get => forcePowersInitialized;
            set => forcePowersInitialized = value;
        }

        public int Level
        {
            get => level;
            set => level = value;
        }

        public int XP
        {
            get => xp;
            set => xp = value;
        }

        public int AbilityPoints
        {
            get => abilityPoints;
            set => abilityPoints = value;
        }

        public int TicksUntilXPGain
        {
            get => ticksUntilXPGain;
            set => ticksUntilXPGain = value;
        }

        public int TicksUntilMeditate
        {
            get => ticksUntilMeditate;
            set => ticksUntilMeditate = value;
        }

        public int TicksAffiliation
        {
            get => ticksAffiliation;
            set => ticksAffiliation = value;
        }

        public float Alignment
        {
            get => alignment;
            set => alignment = value;
        }

        public Pawn Pawn => pawn;
        public Faction Affiliation { get; set; } = null;


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
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceRage_Apprentice, ProjectJediDefOf.PJ_ForceRage_Adept,
                            ProjectJediDefOf.PJ_ForceRage_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceChoke_Apprentice, ProjectJediDefOf.PJ_ForceChoke_Adept,
                            ProjectJediDefOf.PJ_ForceChoke_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceDrain_Apprentice, ProjectJediDefOf.PJ_ForceDrain_Adept,
                            ProjectJediDefOf.PJ_ForceDrain_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceLightning_Apprentice, ProjectJediDefOf.PJ_ForceLightning_Adept,
                            ProjectJediDefOf.PJ_ForceLightning_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceStorm_Apprentice, ProjectJediDefOf.PJ_ForceStorm_Adept,
                            ProjectJediDefOf.PJ_ForceStorm_Master
                        })
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
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForcePush_Apprentice, ProjectJediDefOf.PJ_ForcePush_Adept,
                            ProjectJediDefOf.PJ_ForcePush_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForcePull_Apprentice, ProjectJediDefOf.PJ_ForcePull_Adept,
                            ProjectJediDefOf.PJ_ForcePull_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceSpeed_Apprentice, ProjectJediDefOf.PJ_ForceSpeed_Adept,
                            ProjectJediDefOf.PJ_ForceSpeed_Master
                        })
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
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceHealingSelf_Apprentice, ProjectJediDefOf.PJ_ForceHealingSelf_Adept,
                            ProjectJediDefOf.PJ_ForceHealingSelf_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceHealingOther_Apprentice,
                            ProjectJediDefOf.PJ_ForceHealingOther_Adept, ProjectJediDefOf.PJ_ForceHealingOther_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceDefense_Apprentice, ProjectJediDefOf.PJ_ForceDefense_Adept,
                            ProjectJediDefOf.PJ_ForceDefense_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_MindTrick_Apprentice, ProjectJediDefOf.PJ_MindTrick_Adept,
                            ProjectJediDefOf.PJ_MindTrick_Master
                        }),
                        new ForcePower(new List<AbilityDef>
                        {
                            ProjectJediDefOf.PJ_ForceGhost_Apprentice, ProjectJediDefOf.PJ_ForceGhost_Adept,
                            ProjectJediDefOf.PJ_ForceGhost_Master
                        })
                    };
                }

                return powersLight;
            }
        }

        public IEnumerable<ForcePower> Powers => PowersDark.Concat(PowersGray.Concat(PowersLight));

        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "forceDataPawn");
            Scribe_Values.Look(ref alignment, "forceDataAlignment", 0.5f);
            Scribe_Values.Look(ref level, "forceDataLevel");
            Scribe_Values.Look(ref xp, "forceDataXp");
            Scribe_Values.Look(ref forcePowersInitialized, "forceDataPowersInitialized", true);
            Scribe_Values.Look(ref abilityPoints, "forceDataAbilityPoints");
            Scribe_Values.Look(ref ticksUntilMeditate, "forceDataTicksUntilMeditate");
            Scribe_Values.Look(ref ticksUntilXPGain, "forceDataTicksUntilXPGain", -1);
            Scribe_Values.Look(ref ticksAffiliation, "forceDataTicksAffiliation", -1);
            Scribe_Collections.Look(ref skills, "forceDataSkills", LookMode.Deep);
            Scribe_Collections.Look(ref powersDark, "forceDataPowersDark", LookMode.Deep);
            Scribe_Collections.Look(ref powersGray, "forceDataPowersGray", LookMode.Deep);
            Scribe_Collections.Look(ref powersLight, "forceDataPowersLight", LookMode.Deep);
        }
    }
}