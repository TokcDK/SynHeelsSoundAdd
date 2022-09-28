using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using SynHeelsSoundAdd.Types;
using SynHeelsSoundAdd.Types.NifFileType;

namespace SynHeelsSoundAdd
{
    public class Program
    {
        public static Lazy<Settings> PatchSettings = null!;
        internal static Mutagen.Bethesda.Plugins.Cache.ILinkCache<ISkyrimMod, ISkyrimModGetter>? LinkCache;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out PatchSettings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynHeelsSoundAdd.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // get heels sound formkey
            if (!PatchSettings.Value.FootstepSoundSet.TryResolve(state.LinkCache, out var footstepSoundSetFormKey))
            {
                Console.WriteLine($"Failed to get heels sound for {PatchSettings.Value.FootstepSoundSet.FormKey}! Missing mod?");
                return;
            }

            LinkCache = state.LinkCache;

            // get clothing only option
            bool isOnlyClothing = PatchSettings.Value.IsAddForClothingOnly;

            // set types
            var types = new List<TypeBase>(2)
            {
                new ScriptType(),
                new NifFileType(),
            };

            // set type valid and using data
            foreach (var type in types)
                if (type.SetIsValid(state))
                    type.SetInputData(new TypeData() { State = state, HighHeelSoundFormKey = footstepSoundSetFormKey.FormKey });

            // check all boots with types
            foreach (var armorGetter in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                // skip invalid
                if (armorGetter == null) continue;
                if (armorGetter.BodyTemplate == null) continue;
                if (!armorGetter.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Feet)) continue; // only boots
                //// option: skip armored boots
                if (isOnlyClothing && (armorGetter.BodyTemplate.ArmorType != ArmorType.Clothing)) continue;

                foreach (var type in types)
                {
                    if (type.IsFound(armorGetter))
                    {
                        type.AddSound();
                        break; // sound added
                    }
                }
            }

            Console.WriteLine("Finished");
        }
    }
}
