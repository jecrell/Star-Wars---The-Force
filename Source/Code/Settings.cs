using Verse;

namespace ProjectJedi
{
    public class Settings : ModSettings
    {
        public float forceXPDelayFactor = 1;
        public string forceXPDelayStringBuffer;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref forceXPDelayFactor, "forceXPDelayFactor");
        }
    }
}