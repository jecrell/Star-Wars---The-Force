using UnityEngine;
using Verse;

namespace ProjectJedi
{
    public class ModMain : Mod
    {
        Settings settings;

        public ModMain(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Settings>();
            ModInfo.forceXPDelayFactor = this.settings.forceXPDelayFactor;
        }

        public override string SettingsCategory() => "Star Wars - The Force";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var label = "PJ_SettingsForceXPDelay_Num".Translate(this.settings.forceXPDelayFactor);
            this.settings.forceXPDelayFactor = Widgets.HorizontalSlider(inRect.TopHalf().TopHalf().TopHalf(), this.settings.forceXPDelayFactor, 0.1f, 10f, false, label, null, null, 0.1f);

            this.WriteSettings();

        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            ModInfo.forceXPDelayFactor = this.settings.forceXPDelayFactor;
        }

    }

    public class Settings : ModSettings
    {
        public float forceXPDelayFactor = 1;
        public string forceXPDelayStringBuffer;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.forceXPDelayFactor, "forceXPDelayFactor", 0);
        }
    }
}
