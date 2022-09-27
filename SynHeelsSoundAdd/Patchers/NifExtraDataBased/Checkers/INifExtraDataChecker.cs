using nifly;
using static nifly.niflycpp;

namespace SynHeelsSoundAdd.Patchers.NifExtraDataBased.Checkers
{
    public interface INifExtraDataChecker
    {
        public bool IsValid(NiBlockRefNiExtraData extraDataRef, BlockCache blockCache);
    }
}
