using System;

using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Restore;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Struct for restore-build settings
    /// </summary>
    public readonly struct DotNetCoreRestoreBuildSettings
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DotNetCoreRestoreBuildSettings"/>
        /// </summary>
        /// <param name="restoreSettings"></param>
        /// <param name="buildSettings"></param>
        public DotNetCoreRestoreBuildSettings(
            DotNetCoreRestoreSettings restoreSettings,
            DotNetCoreBuildSettings buildSettings)
        {
            RestoreSettings = restoreSettings
                ?? throw new ArgumentNullException(nameof(restoreSettings));

            BuildSettings = buildSettings
                ?? throw new ArgumentNullException(nameof(buildSettings));
        }

        /// <summary>
        /// Gets the <see cref="DotNetCoreRestoreSettings"/>
        /// </summary>
        public DotNetCoreRestoreSettings RestoreSettings { get; }


        /// <summary>
        /// Gets the <see cref="DotNetCoreBuildSettings"/>
        /// </summary>
        public DotNetCoreBuildSettings BuildSettings { get; }
    }
}
