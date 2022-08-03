using Mutagen.Bethesda.Synthesis.Settings;

namespace SynHeelsSoundAdd
{
    public class Settings
    {
        [SynthesisSettingName("Add only for clothing")]
        [SynthesisTooltip("Enable if want heels sound to be added only for clothing hh boots and not for armored")]
        [SynthesisDescription("Determines if need to check only clothing boots and skip armors")]
        public bool IsAddForClothingOnly { get; set; } = false;
    }
}