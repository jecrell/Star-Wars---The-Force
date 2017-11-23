using System;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    [StaticConstructorOnStartup]
    public static class TexButton
    {
        public static readonly Texture2D PJTex_AlignmentBar = ContentFinder<Texture2D>.Get("UI/AlignmentTexture", true);
        public static readonly Texture2D PJTex_AlignmentBarMarker = ContentFinder<Texture2D>.Get("UI/AlignmentBarMarker", true);

        public static readonly Texture2D PJTex_SkillBox = ContentFinder<Texture2D>.Get("UI/SkillsBox", true);
        public static readonly Texture2D PJTex_SkillBoxAdd = ContentFinder<Texture2D>.Get("UI/SkillsBoxAdd", true);
        public static readonly Texture2D PJTex_SkillBoxFull = ContentFinder<Texture2D>.Get("UI/SkillsBoxFull", true);

        //Light Side
        public static readonly Texture2D PJTex_MindTrick = ContentFinder<Texture2D>.Get("UI/ForceUser/MindTrick", true);
        public static readonly Texture2D PJTex_ForceHeal = ContentFinder<Texture2D>.Get("UI/ForceUser/HealMinor", true);
        public static readonly Texture2D PJTex_ForceHealOther = ContentFinder<Texture2D>.Get("UI/ForceUser/HealOther", true);
        public static readonly Texture2D PJTex_ForceDefense = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceDefense", true);
        public static readonly Texture2D PJTex_ForceGhost = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceGhost", true);

        //Gray Side
        public static readonly Texture2D PJTex_ForcePull = ContentFinder<Texture2D>.Get("UI/ForceUser/ForcePull", true);
        public static readonly Texture2D PJTex_ForcePush = ContentFinder<Texture2D>.Get("UI/ForceUser/ForcePush", true);
        public static readonly Texture2D PJTex_ForceSpeed = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceSpeed", true);

        public static readonly Texture2D PJTex_ForcePointLight = ContentFinder<Texture2D>.Get("UI/ForcePointLight", true);
        public static readonly Texture2D PJTex_ForcePointGray = ContentFinder<Texture2D>.Get("UI/ForcePointGray", true);
        public static readonly Texture2D PJTex_ForcePointDark = ContentFinder<Texture2D>.Get("UI/ForcePointDark", true);
        public static readonly Texture2D PJTex_ForcePointDim = ContentFinder<Texture2D>.Get("UI/ForcePointDim", true);


        //Dark Side
        public static readonly Texture2D PJTex_ForceRage = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceRage", true);
        public static readonly Texture2D PJTex_ForceDrain = ContentFinder<Texture2D>.Get("UI/ForceUser/DrainMinor", true);
        public static readonly Texture2D PJTex_ForceChoke = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceChoke", true);
        public static readonly Texture2D PJTex_ForceLightning = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceLightning", true);
        public static readonly Texture2D PJTex_ForceStorm = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceStorm", true);

    }
}