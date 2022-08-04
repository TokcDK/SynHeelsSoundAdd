using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace SynHeelsSoundAdd.Patchers
{
    public class PatcherData
    {
        public IPatcherState<ISkyrimMod, ISkyrimModGetter>? State;
        public FormKey HighHeelSoundFormKey;
    }

    public abstract class PatcherBase
    {
        protected PatcherData? Data;
        bool IsValid = false;

        public bool SetIsValid(IPatcherState<ISkyrimMod, ISkyrimModGetter> state) { return IsValid = MeValid(state); }

        protected virtual bool MeValid(IPatcherState<ISkyrimMod, ISkyrimModGetter> state) { return true; }

        public void SetInputData(PatcherData data) { Data = data; }

        protected IArmorAddonGetter? ArmorAddon;
        protected IArmorGetter? Armor;

        protected abstract string Name { get; }
        public bool IsFound(IArmorGetter armor)
        {
            if (!IsValid) return false;

            Armor = armor;

            if (CheckIfFound(out IArmorAddonGetter? armorAddon))
            {
                if (ArmorAddon == null && armorAddon != null) ArmorAddon = armorAddon;
                return true;
            }

            return false;
        }

        protected abstract bool CheckIfFound(out IArmorAddonGetter? armorAddon);

        internal void AddSound()
        {
            if (ArmorAddon == null)
            {
                Console.WriteLine($"'{Name}' returned True but Armor Addon is Null!");
                return;
            }

            var armorReportName = Armor == null ? $"'{ArmorAddon!.EditorID}|{ArmorAddon!.FormKey}'" : $"'{Armor!.EditorID}|{Armor!.FormKey}'";
            Console.WriteLine($"Set heels sound for {armorReportName} using '{Name}'");
            Data!.State!.PatchMod.ArmorAddons.GetOrAddAsOverride(ArmorAddon!).FootstepSound.FormKey = Data.HighHeelSoundFormKey;
        }
    }
}
