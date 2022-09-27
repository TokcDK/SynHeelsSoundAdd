﻿using nifly;
using static nifly.niflycpp;
using System.Text.RegularExpressions;

namespace SynHeelsSoundAdd.Patchers.NifExtraDataBased.Checkers
{
    public class NIOHHChecker : INifExtraDataChecker
    {
        public bool IsValid(NiBlockRefNiExtraData extraDataRef, BlockCache blockCache)
        {
            var floatExtraData = blockCache.EditableBlockById<NiStringExtraData>(extraDataRef.index);
            if (floatExtraData == null) return false;

            using var name = floatExtraData.name;
            using var value = floatExtraData.stringData;

            if (name.get() != "SDTA") return false; // check if HH_OFFSET

            Match match = Regex.Match(value.get(), @"\[{\""name\"":\s*\""NPC\"",\s*\""pos\"":\s*\[0,\s*0,\s*([0-9\.]+)\]}\]");
            if (!match.Success) return false; // check if success found json string for offset value
            if (!float.TryParse(match.Groups[1].Value, out var offset)) return false;

            if (offset < 4.0) return false; // check if valid offset value

            return true;
        }
    }
}
