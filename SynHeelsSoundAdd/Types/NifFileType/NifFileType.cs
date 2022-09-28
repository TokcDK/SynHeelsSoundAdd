using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynHeelsSoundAdd.Types.NifFileType.Tools;

namespace SynHeelsSoundAdd.Types.NifFileType
{
    internal class NifFileType : TypeBase
    {
        protected override string Name => "Nif extradata reader";

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
