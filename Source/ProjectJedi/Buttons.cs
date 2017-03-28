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
    }
}