using System;

using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Restore;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Struct for restore-publish settings
    /// </summary>
    public readonly struct DotNetCoreRestorePublishSettings
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DotNetCoreRestorePublishSettings"/>
        /// </summary>
        /// <param name="restoreSettings"></param>
        /// <param name="publishSettings"></param>
        public DotNetCoreRestorePublishSettings(
            DotNetCoreRestoreSettings restoreSettings,
            DotNetCorePublishSettings publishSettings)
        {
            RestoreSettings = restoreSettings
                ?? throw new ArgumentNullException(nameof(restoreSettings));

            PublishSettings = publishSettings
                ?? throw new ArgumentNullException(nameof(publishSettings));
        }


        /// <summary>
        /// Gets the <see cref="DotNetCoreRestoreSettings"/>
        /// </summary>
        public DotNetCoreRestoreSettings RestoreSettings { get; }


        /// <summary>
        /// Gets the <see cref="DotNetCorePublishSettings"/>
        /// </summary>
        public DotNetCorePublishSettings PublishSettings { get; }
    }
}
