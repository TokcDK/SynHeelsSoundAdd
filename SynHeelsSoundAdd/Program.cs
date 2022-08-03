using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace SynHeelsSoundAdd
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        static FormKey HighHeelSoundFormKey;
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // get heels sound formkey
            if (!FormKey.TryFactory("004527:Heels Sound.esm", out var formKey))
            {
                Console.WriteLine("Failed to get heels sound! Exit..");
                return;
            }

            HighHeelSoundFormKey = formKey;

            AddByHdtHighHeelScript(state);

            Console.WriteLine("Finished");
        }

        private static void AddByHdtHighHeelScript(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!state.LoadOrder.ContainsKey("hdtHighHeel.esm"))
            {
                Console.WriteLine("'hdtHighHeel.esm' doeasnt exist! Skip..");
                return;
            }

            foreach (var armorGetter in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                // skip invalid
                if (armorGetter == null) continue;
                if (armorGetter.BodyTemplate == null || !armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) continue; // only boots
                //// check if hdthighheelshoes script
                if (armorGetter.VirtualMachineAdapter == null) continue;
                if (armorGetter.VirtualMachineAdapter.Scripts == null) continue;
                if (armorGetter.VirtualMachineAdapter.Scripts.Count == 0) continue;
                if (!armorGetter.VirtualMachineAdapter.Scripts.Any(sc => sc.Name.ToLowerInvariant() == "hdthighheelshoes")) continue;
                //// check if armor addon has this heel sound
                if (armorGetter.Armature == null) continue;
                IFormLinkGetter<IArmorAddonGetter>? armorAddonReference = armorGetter.Armature.First();
                if (armorAddonReference == null || !armorAddonReference.TryResolve(state.LinkCache, out var armorAddon)) continue;
                if (armorAddon.FootstepSound.FormKey == HighHeelSoundFormKey) continue; // already has the sound

                Console.WriteLine($"Set heels sound for {armorGetter.EditorID}|{armorGetter.FormKey}");
                state.PatchMod.ArmorAddons.GetOrAddAsOverride(armorAddon).FootstepSound.FormKey = HighHeelSoundFormKey;
            }
        }
    }
}
