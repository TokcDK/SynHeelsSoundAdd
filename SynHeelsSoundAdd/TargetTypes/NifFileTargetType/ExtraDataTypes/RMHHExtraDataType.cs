using nifly;
using static nifly.niflycpp;

namespace SynHeelsSoundAdd.TargetTypes.NifFileTargetType.ExtraDataTypes
{
    public class RMHHExtraDataType : IExtraDataTypeBase
    {
        public bool IsValid(NiBlockRefNiExtraData extraDataRef, BlockCache blockCache)
        {
            var floatExtraData = blockCache.EditableBlockById<NiFloatExtraData>(extraDataRef.index);
            if (floatExtraData == null) return false;

            using var name = floatExtraData.name;

            if (name.get() != "HH_OFFSET") return false; // check if HH_OFFSET
            if (Program.PatchSettings.Value.MinOffsetValue <= 0) return true; // dont need to check effect offset when it 0
            if (floatExtraData.floatData < Program.PatchSettings.Value.MinOffsetValue) return false; // check if valid offset value

            return true;
        }
    }
}
