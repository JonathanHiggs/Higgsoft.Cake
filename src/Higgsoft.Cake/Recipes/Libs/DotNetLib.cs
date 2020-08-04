using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Common.Tools.NuGet.Push;
using Cake.Core.IO;

using Higgsoft.Cake.Utils;

namespace Higgsoft.Cake.Recipes.Libs
{
    /// <summary>
    /// Recipe for building a dotnet library nuget package
    /// </summary>
    public class DotNetLib : Recipe
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
        /// Gets the <see cref="DotNetLibTasks"/>
        /// </summary>
        public DotNetLibTasks Tasks { get; } = new DotNetLibTasks();


        #region Paths

        /// <summary>
        /// Gets and sets the path to the publish directory
        /// </summary>
        public DirectoryPath PublishDirectory { get; set; }


        /// <summary>
        /// Gets and sets the path to the nuget directory
        /// </summary>
        public DirectoryPath NuGetDirectory { get; set; }

        #endregion


        #region NuGet Settings

        /// <summary>
        /// Gets the list of package authors
        /// </summary>
        public List<string> Authors { get; } = new List<string>();


        /// <summary>
        /// Adds the supplied authors to the list of package authors
        /// </summary>
        /// <param name="authors"></param>
        public void AddAuthors(params string[] authors)
            => Authors.AddRange(authors);


        /// <summary>
        /// Gets and sets the link to the package website
        /// </summary>
        public Uri ProjectUrl { get; set; }


        /// <summary>
        /// Gets and sets the link to the package icon
        /// </summary>
        public Uri IconUrl { get; set; }


        /// <summary>
        /// Gets a list of package tags
        /// </summary>
        public List<string> Tags { get; }


        /// <summary>
        /// Adds the supplied tags to the list of package tags
        /// </summary>
        /// <param name="tags"></param>
        public void AddTags(params string[] tags)
            => Tags.AddRange(tags);


        /// <summary>
        /// Gets a tag that determines whether debug symbols are included with the package
        /// </summary>
        public bool Symbols { get; set; } = false;


        /// <summary>
        /// Gets the list of nuget content definitions
        /// </summary>
        public List<NuSpecContent> NuGetFiles { get; } = new List<NuSpecContent>();


        /// <summary>
        /// Adds the supplied files to the list of nuget content definitions
        /// </summary>
        /// <param name="files"></param>
        public void AddNuGetFiles(params NuSpecContent[] files)
            => NuGetFiles.AddRange(files);


        /// <summary>
        /// Gets the list of nuget package dependencies
        /// </summary>
        public List<NuSpecDependency> NuSpecDependencies { get; } = new List<NuSpecDependency>();


        /// <summary>
        /// Adds the supplied dependencies to the list of nuget package dependencies
        /// </summary>
        /// <param name="dependencies"></param>
        public void AddDependencies(params NuSpecDependency[] dependencies)
            => NuSpecDependencies.AddRange(dependencies);

        #endregion


        #region Generated

        /// <summary>
        /// Gets the <see cref="DotNetCoreMSBuildSettings"/> for the recipe
        /// </summary>
        public DotNetCoreMSBuildSettings MSBuildSettings
            => new DotNetCoreMSBuildSettings { };


        /// <summary>
        /// Gets a sequence of restore-build settings
        /// </summary>
        public IEnumerable<DotNetCoreRestoreBuildSettings> RestoreBuildSettings
            => Frameworks.Select(framework =>
                new DotNetCoreRestoreBuildSettings(
                    new DotNetCoreRestoreSettings {
                        MSBuildSettings = MSBuildSettings,
                        Verbosity = Build.Verbosity.ToDotNetCoreVerbosity() },
                    new DotNetCoreBuildSettings {
                        Configuration = Build.Configuration,
                        NoRestore = true,
                        MSBuildSettings = MSBuildSettings,
                        Framework = framework }));


        /// <summary>
        /// Gets the <see cref="DotNetCorePublishSettings"/> for the recipe
        /// </summary>
        public IEnumerable<DotNetCoreRestorePublishSettings> RestorePublishSettings
            => Frameworks.Select(framework =>
                new DotNetCoreRestorePublishSettings(
                    new DotNetCoreRestoreSettings {
                        MSBuildSettings = MSBuildSettings,
                        Verbosity = Build.Verbosity.ToDotNetCoreVerbosity() },
                    new DotNetCorePublishSettings {
                        Configuration = Build.Configuration,
                        NoRestore = true,
                        Framework = framework,
                        MSBuildSettings = MSBuildSettings,
                        OutputDirectory = $"{PublishDirectory}\\{framework}",
                        Verbosity = Build.Verbosity.ToDotNetCoreVerbosity() }));


        /// <summary>
        /// Gets the <see cref="Pack.NuGetPackSettings"/> for the recipe
        /// </summary>
        public NuGetPackSettings NuGetPackSettings
            => new NuGetPackSettings {
                Id = Id,
                Title = Name,
                Version = Version.ToString(),
                Authors = Authors,
                Owners = new[] { Build.Company },
                Description = Description,
                Summary = Description,
                ReleaseNotes = ReleaseNotes,
                ProjectUrl = ProjectUrl,
                IconUrl = IconUrl,
                Copyright = $"Copyright (c) {Build.Company} {DateTime.Today.Year}",
                Tags = Tags,
                Symbols = Symbols,
                Files = NuGetFiles,
                BasePath = PublishDirectory,
                OutputDirectory = NuGetDirectory,
                RequireLicenseAcceptance = false,
                Properties = new Dictionary<string, string> {
                    { "Configuration", Build.Configuration } },
                Dependencies = NuSpecDependencies,
                Verbosity = Build.Verbosity.ToNuGetVerbosity()
            };


        /// <summary>
        /// Gets the <see cref="Push.NuGetPushSettings"/> for the recipe
        /// </summary>
        public NuGetPushSettings NuGetPushSettings
            => Build.Local
            ? new NuGetPushSettings { Source = Build.NuGetLocalSource }
            : new NuGetPushSettings {
                Source = Build.NuGetSource,
                ApiKey = Build.NuGetApiKey,
                Verbosity = Build.Verbosity.ToNuGetVerbosity()
            };

        #endregion
    }
}
