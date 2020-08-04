using System;
using System.Collections.Generic;
using System.Linq;

using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Core.IO;

using Higgsoft.Cake.Utils;

namespace Higgsoft.Cake.Recipes.Apps
{
    /// <summary>
    /// Recipe for building a dotnet application
    /// </summary>
    public class DotNetApp : Recipe
    {
        /// <summary>
        /// Gets a list of target frameworks
        /// </summary>
        public List<string> Frameworks { get; } = new List<string>();


        /// <summary>
        /// Adds the supplied frameworks to the list of target frameworks
        /// </summary>
        /// <param name="frameworks"></param>
        public void AddFrameworks(params string[] frameworks)
            => Frameworks.AddRange(frameworks);


        /// <summary>
        /// Gets a list of target runtimes
        /// </summary>
        public List<string> Runtimes { get; } = new List<string>();


        /// <summary>
        /// Adds the supplied runtimes to the list of target runtimes
        /// </summary>
        /// <param name="runtimes"></param>
        public void AddRuntimes(params string[] runtimes)
            => Runtimes.AddRange(runtimes);


        /// <summary>
        /// Gets a sequence of runtime-framework pairs
        /// </summary>
        public IEnumerable<(string Runtime, string Framework)> RuntimeFrameworks
            => Runtimes.SelectMany(r => Frameworks.Select(f => (r, f)));


        /// <summary>
        /// Gets the <see cref="DotNetAppTasks"/>
        /// </summary>
        public DotNetAppTasks Tasks { get; } = new DotNetAppTasks();


        #region Paths

        /// <summary>
        /// Gets and sets the path to the publish directory
        /// </summary>
        public DirectoryPath PublishDirectory { get; set; }


        /// <summary>
        /// Gets and sets the path to the package directory
        /// </summary>
        public DirectoryPath PackageDirectory { get; set; }


        /// <summary>
        /// Gets and sets the path to the artefacts directory
        /// </summary>
        public DirectoryPath ArtefactDirectory { get; set; }

        #endregion


        #region Generated

        /// <summary>
        /// Gets the <see cref="DotNetCoreMSBuildSettings"/> for the recipe
        /// </summary>
        public DotNetCoreMSBuildSettings MSBuildSettings
            => new DotNetCoreMSBuildSettings { };


        /// <summary>
        /// Gets a sequence of restore-build settings for runtime-framework pairs
        /// </summary>
        public IEnumerable<DotNetCoreRestoreBuildSettings>
            RestoreBuildSettings
            => RuntimeFrameworks.Select(p => new DotNetCoreRestoreBuildSettings(
                new DotNetCoreRestoreSettings {
                    MSBuildSettings = MSBuildSettings,
                    Verbosity = Build.Verbosity.ToDotNetCoreVerbosity(),
                    Runtime = p.Runtime },
                new DotNetCoreBuildSettings {
                    Configuration = Build.Configuration,
                    NoRestore = true,
                    MSBuildSettings = MSBuildSettings,
                    Runtime = p.Runtime,
                    Framework = p.Framework }));


        /// <summary>
        /// Gets a sequence of restore-publish settings for runtime-framework pairs
        /// </summary>
        public IEnumerable<DotNetCoreRestorePublishSettings>
            PublishSettings
            => RuntimeFrameworks.Select(p => new DotNetCoreRestorePublishSettings(
                new DotNetCoreRestoreSettings {
                    MSBuildSettings = MSBuildSettings,
                    Verbosity = Build.Verbosity.ToDotNetCoreVerbosity(),
                    Runtime = p.Runtime },
                new DotNetCorePublishSettings {
                    NoRestore = true,
                    Framework = p.Framework,
                    MSBuildSettings = MSBuildSettings,
                    OutputDirectory = $"{PublishDirectory}\\{p.Framework}\\{p.Runtime}",
                    Verbosity = Build.Verbosity.ToDotNetCoreVerbosity() }));


        /// <summary>
        /// Gets the temporary artefact file path
        /// </summary>
        public FilePath TempArtefactFile
            => new FilePath($"{PublishDirectory}\\{Id}.{Version}.zip");


        /// <summary>
        /// Gets the artefact file path
        /// </summary>
        public FilePath ArtefactFile
            => new FilePath($"{ArtefactDirectory}\\{Id}.{Version}.zip");

        #endregion


        #region Helpers

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

        #endregion
    }
}
