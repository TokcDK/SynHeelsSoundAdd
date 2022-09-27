using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;

namespace SynHeelsSoundAdd.Patchers
{
    internal class NIOHH : PatcherBase
    {
        protected override string Name => "NIOHH patcher";

        protected override bool IsValidArmor()
        {
            return true;
        }

        protected override bool IsValidArmorAddon()
        {
            if (ArmorAddon!.WorldModel == null) return false;
            if (ArmorAddon.WorldModel.Female == null) return false;

            var fileSubPath = ArmorAddon.WorldModel.Female.File;
            if (string.IsNullOrWhiteSpace(fileSubPath)) return false;

            var filePath = Data!.State!.DataFolderPath + "\\meshes\\" + fileSubPath;
            if (!File.Exists(filePath)) return false;

            if (!IsFoundValidMarker(filePath)) return false;

            return true;
        }

        // examples of using: https://github.com/SteveTownsend/AllGUDMeshGen
        private static bool IsFoundValidMarker(string filePath)
        {
            var loadOptions = new NifLoadOptions { isTerrain = false };
            var nifFile = new NifFile();

            var loadResult = nifFile.Load(filePath, loadOptions);
            if (loadResult != 0) return false; // nif cant be loaded

            var blockCache = new niflycpp.BlockCache(nifFile.GetHeader());
            var shapes = nifFile.GetShapes();
            foreach (var shape in shapes)
            {
                foreach (var extraDataRef in shape.extraDataRefs.GetRefs())
                {
                    using (extraDataRef)
                    {
                        if (extraDataRef.IsEmpty()) continue;

                        var floatExtraData = blockCache.EditableBlockById<NiFloatExtraData>(extraDataRef.index);
                        if (floatExtraData == null) continue;

                        using var name = floatExtraData.name;

                        if (name.get() != "HH_OFFSET") continue; // check if HH_OFFSET
                        if (floatExtraData.floatData < 4) return false; // check if valid offset value

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
