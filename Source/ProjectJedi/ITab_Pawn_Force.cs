using RimWorld;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ITab_Pawn_Force : ITab
    {
        public ITab_Pawn_Force()
        {
            size = ForceCardUtility.ForceCardSize + (new Vector2(17f, 17f) * 2f);
            labelKey = "PJ_TabForce";
        }

        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                if (SelPawn != null)
                {
                    pawn = SelPawn;
                }
                else
                {
                    if (SelThing is Corpse corpse)
                    {
                        pawn = corpse.InnerPawn;
                    }
                }

                if (pawn != null)
                {
                    return pawn;
                }

                Log.Error("Character tab found no selected pawn to display.");
                return null;
            }
        }

        public override bool IsVisible
        {
            get
            {
                if (SelPawn.story != null)
                {
                    return SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                           SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_GrayTrait) ||
                           SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait) ||
                           SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_ForceSensitive);
                }

                return false;
            }
        }

        protected override void FillTab()
        {
            var rect = new Rect(17f, 17f, ForceCardUtility.ForceCardSize.x, ForceCardUtility.ForceCardSize.y);
            ForceCardUtility.DrawForceCard(rect, PawnToShowInfoAbout);
        }
    }
}