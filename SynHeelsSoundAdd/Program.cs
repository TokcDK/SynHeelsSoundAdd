using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace SynHeelsSoundAdd
{
    public class Program
    {
        public static Lazy<Settings> PatchSettings = null!;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out PatchSettings)
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
            AddByNIOHighHeelMesh(state);

            Console.WriteLine("Finished");
        }

        private static void AddByNIOHighHeelMesh(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            bool isOnlyClothing = PatchSettings.Value.IsAddForClothingOnly;
            foreach (var armorGetter in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                // skip invalid
                if (armorGetter == null) continue;
                if (armorGetter.BodyTemplate == null) continue;
                if (!armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) continue; // only boots
                //// option: skip armored boots
                if (isOnlyClothing && (armorGetter.BodyTemplate.ArmorType != ArmorType.Clothing)) continue;
                //// check if armor addon has this heel sound
                if (armorGetter.Armature == null) continue;

                // search boots armor addon
                IArmorAddonGetter? armorAddon = null;
                foreach (var aaFormlinkGetter in armorGetter.Armature)
                {
                    // skip all armor addons except boots
                    if (!aaFormlinkGetter.TryResolve(state.LinkCache, out var aa)) continue;
                    if (aa.BodyTemplate == null) continue;
                    if (!aa.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) continue;

                    armorAddon = aa;
                    break;
                }

                if (armorAddon == null) continue; // boots armor addon not found
                if (armorAddon.WorldModel==null) continue;
                if (armorAddon.WorldModel.Female==null) continue;

                var fileSubPath = armorAddon.WorldModel.Female.File;
                if (string.IsNullOrWhiteSpace(fileSubPath)) continue;

                var filePath = state.DataFolderPath +"\\meshes\\" + fileSubPath;
                if (!File.Exists(filePath)) continue;

                // just search string
                var fileString = File.ReadAllText(filePath);
                if (!fileString.Contains("[{\"name\":\"NPC\",\"pos\":[", StringComparison.InvariantCulture)) continue;

                Console.WriteLine($"Set heels sound for {armorGetter.EditorID}|{armorGetter.FormKey}");
                state.PatchMod.ArmorAddons.GetOrAddAsOverride(armorAddon).FootstepSound.FormKey = HighHeelSoundFormKey;
            }
        }

        private static void AddByHdtHighHeelScript(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!state.LoadOrder.ContainsKey("hdtHighHeel.esm"))
            {
                Console.WriteLine("'hdtHighHeel.esm' doeasnt exist! Skip..");
                return;
            }

            bool isOnlyClothing = PatchSettings.Value.IsAddForClothingOnly;
            foreach (var armorGetter in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                // skip invalid
                if (armorGetter == null) continue;
                if (armorGetter.BodyTemplate == null) continue;
                if (!armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) continue; // only boots
                //// option: skip armored boots
                if (isOnlyClothing && (armorGetter.BodyTemplate.ArmorType != ArmorType.Clothing)) continue;
                //// check if 'hdthighheelshoes' script
                if (armorGetter.VirtualMachineAdapter == null) continue;
                if (armorGetter.VirtualMachineAdapter.Scripts == null) continue;
                if (armorGetter.VirtualMachineAdapter.Scripts.Count == 0) continue;
                if (!armorGetter.VirtualMachineAdapter.Scripts.Any(sc => sc.Name.ToLowerInvariant() == "hdthighheelshoes")) continue;
                //// check if armor addon has this heel sound
                if (armorGetter.Armature == null) continue;

                // search boots armor addon
                IArmorAddonGetter? armorAddon = null;
                foreach (var aaFormlinkGetter in armorGetter.Armature)
                {
                    // skip all armor addons except boots
                    if (!aaFormlinkGetter.TryResolve(state.LinkCache, out var aa)) continue;
                    if (aa.BodyTemplate == null) continue;
                    if (!aa.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) continue;

                    armorAddon = aa;
                    break;
                }

                if (armorAddon == null) continue; // boots armor addon not found
                if (armorAddon.FootstepSound.FormKey == HighHeelSoundFormKey) continue; // already has the sound

                Console.WriteLine($"Set heels sound for {armorGetter.EditorID}|{armorGetter.FormKey}");
                state.PatchMod.ArmorAddons.GetOrAddAsOverride(armorAddon).FootstepSound.FormKey = HighHeelSoundFormKey;
            }
        }
    }
}
