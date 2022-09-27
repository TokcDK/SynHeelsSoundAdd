using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace SynHeelsSoundAdd.Patchers
{
    internal class HDTHighHeelsReader : ReaderBase
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

        protected override bool IsValidArmor()
        {
            if (Armor!.VirtualMachineAdapter == null) return false;
            if (Armor.VirtualMachineAdapter.Scripts == null) return false;
            if (Armor.VirtualMachineAdapter.Scripts.Count == 0) return false;
            if (!Armor.VirtualMachineAdapter.Scripts.Any(sc => string.Equals(sc.Name, "hdthighheelshoes", StringComparison.InvariantCultureIgnoreCase))) return false;

            return true;
        }

        protected override bool IsValidArmorAddon() { return true; }
    }
}
