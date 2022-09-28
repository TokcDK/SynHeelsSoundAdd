using nifly;
using static nifly.niflycpp;

namespace SynHeelsSoundAdd.Types.NifFileType.ExtraDataTargets
{
    public interface IExtraDataTargetBase
    {
        public bool IsValid(NiBlockRefNiExtraData extraDataRef, BlockCache blockCache);
    }
}
