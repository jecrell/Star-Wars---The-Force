using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ITab_Pawn_Force : ITab
    {
        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                if (base.SelPawn != null)
                {
                    pawn = base.SelPawn;
                }
                else
                {
                    Corpse corpse = base.SelThing as Corpse;
                    if (corpse != null)
                    {
                        pawn = corpse.InnerPawn;
                    }
                }
                if (pawn == null)
                {
                    Log.Error("Character tab found no selected pawn to display.");
                    return null;
                }
                return pawn;
            }
        }

        public override bool IsVisible
        {
            get
            {
                if (base.SelPawn.story != null)
                {
                    return (base.SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                        base.SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait) ||
                        base.SelPawn.story.traits.HasTrait(ProjectJediDefOf.PJ_ForceSensitive));
                }
                return false;
            }
        }

        public ITab_Pawn_Force()
        {
            this.size = ForceCardUtility.ForceCardSize + new Vector2(17f, 17f) * 2f;
            this.labelKey = "PJ_TabForce";
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(17f, 17f, ForceCardUtility.ForceCardSize.x, ForceCardUtility.ForceCardSize.y);
            ForceCardUtility.DrawForceCard(rect, this.PawnToShowInfoAbout);
        }
    }
}
