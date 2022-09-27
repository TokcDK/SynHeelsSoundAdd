﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using SynHeelsSoundAdd.Patchers.NifExtraDataBased.Checkers;
using static nifly.niflycpp;

namespace SynHeelsSoundAdd.Patchers.NifExtraDataBased.Tools
{
    public class NiflyTools
    {
        static readonly List<INifExtraDataChecker> CheckersList = new()
        {
            new NIOHHChecker(),
            new RMHHChecker(),
        };

        // examples of using: https://github.com/SteveTownsend/AllGUDMeshGen
        public static bool IsFoundValidMarker(string filePath)
        {
            var nifFile = new NifFile();
            var loadResult = nifFile.Load(filePath, new NifLoadOptions { isTerrain = false });
            if (loadResult != 0) return false; // nif cant be loaded

            var blockCache = new BlockCache(nifFile.GetHeader());
            var shapes = nifFile.GetShapes();
            foreach (var shape in shapes)
            {
                foreach (var extraDataRef in shape.extraDataRefs.GetRefs())
                {
                    using (extraDataRef)
                    {
                        if (extraDataRef.IsEmpty()) continue;

                        foreach (var checker in CheckersList) if (checker.IsValid(extraDataRef, blockCache)) return true;
                    }
                }
            }

            return false;
        }
    }
}
