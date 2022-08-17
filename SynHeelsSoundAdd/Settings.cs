﻿using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis.Settings;

namespace SynHeelsSoundAdd
{
    public class Settings
    {
        [SynthesisSettingName("Add only for clothing")]
        [SynthesisTooltip("Enable if want heels sound to be added only for clothing hh boots and not for armored")]
        [SynthesisDescription("Determines if need to check only clothing boots and skip armors")]
        public bool IsAddForClothingOnly = false;
        [SynthesisTooltip("Footsteps sound set. Default is set set for Heels Sound.esm but you can set it to any.")]
        public FormLink<IFootstepSetGetter> FootstepSoundSet = FormKey.Factory("004527:Heels Sound.esm");
    }
}