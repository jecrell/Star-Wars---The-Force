using Verse;

namespace ProjectJedi
{
    public class Settings : ModSettings
    {
        public float forceXPDelayFactor = 1;
        public float forceWielderDifficultyModifier = 5;
        public string forceXPDelayStringBuffer;
        public static int nonForceUserLightsaberDamage = 10;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref forceWielderDifficultyModifier, "forceWielderDifficultyModifier");
            Scribe_Values.Look(ref forceXPDelayFactor, "forceXPDelayFactor");

        }
    }
}