using UnityEngine;
using Verse;

namespace ProjectJedi
{
    [StaticConstructorOnStartup]
    public static class TexButton
    {
        public static readonly Texture2D PJTex_AlignmentBar = ContentFinder<Texture2D>.Get("UI/AlignmentTexture");

        public static readonly Texture2D PJTex_AlignmentBarMarker =
            ContentFinder<Texture2D>.Get("UI/AlignmentBarMarker");

        public static readonly Texture2D PJTex_SkillBox = ContentFinder<Texture2D>.Get("UI/SkillsBox");
        public static readonly Texture2D PJTex_SkillBoxAdd = ContentFinder<Texture2D>.Get("UI/SkillsBoxAdd");
        public static readonly Texture2D PJTex_SkillBoxFull = ContentFinder<Texture2D>.Get("UI/SkillsBoxFull");

        //Light Side
        public static readonly Texture2D PJTex_MindTrick = ContentFinder<Texture2D>.Get("UI/ForceUser/MindTrick");
        public static readonly Texture2D PJTex_ForceHeal = ContentFinder<Texture2D>.Get("UI/ForceUser/HealMinor");
        public static readonly Texture2D PJTex_ForceHealOther = ContentFinder<Texture2D>.Get("UI/ForceUser/HealOther");
        public static readonly Texture2D PJTex_ForceDefense = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceDefense");
        public static readonly Texture2D PJTex_ForceGhost = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceGhost");

        //Gray Side
        public static readonly Texture2D PJTex_ForcePull = ContentFinder<Texture2D>.Get("UI/ForceUser/ForcePull");
        public static readonly Texture2D PJTex_ForcePush = ContentFinder<Texture2D>.Get("UI/ForceUser/ForcePush");
        public static readonly Texture2D PJTex_ForceSpeed = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceSpeed");

        public static readonly Texture2D PJTex_ForcePointLight = ContentFinder<Texture2D>.Get("UI/ForcePointLight");
        public static readonly Texture2D PJTex_ForcePointGray = ContentFinder<Texture2D>.Get("UI/ForcePointGray");
        public static readonly Texture2D PJTex_ForcePointDark = ContentFinder<Texture2D>.Get("UI/ForcePointDark");
        public static readonly Texture2D PJTex_ForcePointDim = ContentFinder<Texture2D>.Get("UI/ForcePointDim");


        //Dark Side
        public static readonly Texture2D PJTex_ForceRage = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceRage");
        public static readonly Texture2D PJTex_ForceDrain = ContentFinder<Texture2D>.Get("UI/ForceUser/DrainMinor");
        public static readonly Texture2D PJTex_ForceChoke = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceChoke");

        public static readonly Texture2D PJTex_ForceLightning =
            ContentFinder<Texture2D>.Get("UI/ForceUser/ForceLightning");

        public static readonly Texture2D PJTex_ForceStorm = ContentFinder<Texture2D>.Get("UI/ForceUser/ForceStorm");
    }
}