using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynHeelsSoundAdd.Patchers.NifExtraDataBased.Tools;

namespace SynHeelsSoundAdd.Patchers.NifExtraDataBased
{
    internal class NifExtraDataReader : ReaderBase
    {
        protected override string Name => "NIOHH patcher";

        protected override bool IsValidArmor() { return true; }

        protected override bool IsValidArmorAddon()
        {
            if (ArmorAddon!.WorldModel == null) return false;
            if (ArmorAddon.WorldModel.Female == null) return false;

            var fileSubPath = ArmorAddon.WorldModel.Female.File;
            if (string.IsNullOrWhiteSpace(fileSubPath)) return false;

            var filePath = Data!.State!.DataFolderPath + "\\meshes\\" + fileSubPath;
            if (!File.Exists(filePath)) return false;

            if (NiflyTools.IsFoundValidMarker(filePath)) return true;

            return false;
        }
    }
}
