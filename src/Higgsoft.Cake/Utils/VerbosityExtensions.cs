using System;

using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.NuGet;
using Cake.Core.Diagnostics;

namespace Higgsoft.Cake.Utils
{
    /// <summary>
    /// Extension methods for converting between the different verbosity enums
    /// </summary>
    public static class VerbosityExtensions
    {
        /// <summary>
        /// Converts a <see cref="Verbosity"/> to a <see cref="DotNetCoreVerbosity"/>
        /// </summary>
        public static DotNetCoreVerbosity ToDotNetCoreVerbosity(this Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.Diagnostic:  return DotNetCoreVerbosity.Diagnostic;
                case Verbosity.Minimal:     return DotNetCoreVerbosity.Minimal;
                case Verbosity.Normal:      return DotNetCoreVerbosity.Normal;
                case Verbosity.Quiet:       return DotNetCoreVerbosity.Quiet;
                case Verbosity.Verbose:     return DotNetCoreVerbosity.Detailed;
                default:                    throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Converts a <see cref="Verbosity"/> to a <see cref="NuGetVerbosity"/>
        /// </summary>
        public static NuGetVerbosity ToNuGetVerbosity(this Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.Diagnostic:
                case Verbosity.Verbose:
                    return NuGetVerbosity.Detailed;

                case Verbosity.Minimal:
                case Verbosity.Quiet:
                    return NuGetVerbosity.Quiet;

                case Verbosity.Normal:
                    return NuGetVerbosity.Normal;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
