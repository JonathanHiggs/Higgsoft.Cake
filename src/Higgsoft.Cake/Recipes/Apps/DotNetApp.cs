﻿using System.Collections.Generic;
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
        /// Gets the path to the artefacts repository
        /// </summary>
        public DirectoryPath ArtefactsRepository
            => Build.Local
            ? Build.ArtefactsLocalRepository
            : new DirectoryPath($"{Build.ArtefactsRepository}\\{Id}");

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
        public IEnumerable<DotNetCoreRestoreBuildSettings> RestoreBuildSettings
            => RuntimeFrameworks.Select(p =>
                new DotNetCoreRestoreBuildSettings(
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
        public IEnumerable<DotNetCoreRestorePublishSettings> RestorePublishSettings
            => RuntimeFrameworks.Select(p =>
                new DotNetCoreRestorePublishSettings(
                    new DotNetCoreRestoreSettings {
                        MSBuildSettings = MSBuildSettings,
                        Verbosity = Build.Verbosity.ToDotNetCoreVerbosity(),
                        Runtime = p.Runtime },
                    new DotNetCorePublishSettings {
                        Configuration = Build.Configuration,
                        NoRestore = true,
                        Framework = p.Framework,
                        Runtime = p.Runtime,
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
            => new FilePath($"{ArtefactsRepository}\\{Id}.{Version}.zip");

        #endregion


        /// <summary>
        /// Coroutine called when <see cref="Id"/> value is set
        /// </summary>
        /// <param name="id"></param>
        protected override void OnIdChanged(string id)
        {
            Tasks.Names = new DotNetAppTasks.TaskNames(id);
        }
    }
}
