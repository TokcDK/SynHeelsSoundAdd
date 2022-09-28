using nifly;
using static nifly.niflycpp;

namespace SynHeelsSoundAdd.TargetTypes.NifFileTargetType.ExtraDataTypes
{
    public interface IExtraDataTypeBase
    {
        public bool IsValid(NiBlockRefNiExtraData extraDataRef, BlockCache blockCache);
    }
}
