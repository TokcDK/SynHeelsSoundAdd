using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynHeelsSoundAdd.Patchers
{
    internal class HDTHighHeels : PatcherBase
    {
        protected override string Name => "HDTHighHeels patcher";

        protected override bool MeValid(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!state.LoadOrder.ContainsKey("hdtHighHeel.esm"))
            {
                Console.WriteLine("'hdtHighHeel.esm' doeasnt exist! Skip..");
                return false;
            }

            return true;
        }

        protected override bool CheckIfFound(out IArmorAddonGetter? armorAddon)
        {
            armorAddon = null;

            if (Armor!.VirtualMachineAdapter == null) return false;
            if (Armor.VirtualMachineAdapter.Scripts == null) return false;
            if (Armor.VirtualMachineAdapter.Scripts.Count == 0) return false;
            if (!Armor.VirtualMachineAdapter.Scripts.Any(sc => sc.Name.ToLowerInvariant() == "hdthighheelshoes")) return false;
            //// check if armor addon has this heel sound
            if (Armor.Armature == null) return false;

            // search boots armor addon
            ArmorAddon = null;
            foreach (var aaFormlinkGetter in Armor.Armature)
            {
                // skip all armor addons except boots
                if (!aaFormlinkGetter.TryResolve(Data!.State!.LinkCache, out var aa)) return false;
                if (aa.BodyTemplate == null) return false;
                if (!aa.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) return false;

                ArmorAddon = aa;
                break;
            }

            if (ArmorAddon == null) return false; // boots armor addon not found
            if (ArmorAddon.FootstepSound.FormKey == Data!.HighHeelSoundFormKey) return false; // already has the sound

            return true;
        }
    }
}
