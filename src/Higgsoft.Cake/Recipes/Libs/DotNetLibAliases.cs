using System;
using System.Linq;

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.NuGet.Delete;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.List;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.Scripting;
using Cake.Git;

namespace Higgsoft.Cake.Recipes.Libs
{
    /// <summary>
    /// Extension methods for <see cref="DotNetLib"/> recipes
    /// </summary>
    [CakeAliasCategory("Higgsoft.Cake.Recipes.Libs")]
    public static class DotNetLibAliases
    {
        /// <summary>
        /// Configures a <see cref="DotNetLib"/> recipe from a configuration action
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="config">Configuration action</param>
        /// <returns><see cref="DotNetLib"/> recipe settings</returns>
        [CakeMethodAlias]
        public static DotNetLib ConfigDotNetLib(this ICakeContext context, Action<DotNetLib> config)
        {
            var lib = new DotNetLib();
            config(lib);

            if (Build.RecipeBuilds.Any(r => r.Id == lib.Id))
                throw new InvalidOperationException(
                    $"Recipe with id: {lib.Id} already added to the build");

            Build.RecipeBuilds.Add(lib);
            return lib;
        }


        /// <summary>
        /// Logs the recipe settings to the build output
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetLibInfo(this ICakeContext context, DotNetLib lib)
        {
            context.Information("DotNet Lib:");
            context.Information($"Id                        {lib.Id}");
            context.Information($"Name                      {lib.Name}");
            context.Information($"Description               {lib.Description}");
            context.Information($"Solution                  {lib.Solution}");
            context.Information($"Project                   {lib.Project}");
            context.Information("\nPaths");
            context.Information($"Solution Directory        {lib.SolutionDirectory}");
            context.Information($"Solution File             {lib.SolutionFile}");
            context.Information($"Project File              {lib.ProjectFile}");
            context.Information($"Assembly Info File        {lib.AssemblyInfoFile}");
            context.Information($"Release Notes File        {lib.ReleaseNotesFile}");
            context.Information($"Publish Directory         {lib.PublishDirectory}");
            context.Information($"NuGet Directory           {lib.NuGetDirectory}");
            context.Information("\nSettings");
            context.Information($"Prepare Release Notes     {lib.PrepareReleaseNotes}");
            context.Information($"Update Assembly Info      {lib.UpdateAssemblyInfo}");
            context.Information($"Commit Changes            {lib.CommitChanges}");
            context.Information($"Tag Version               {lib.TagVersion}");
            context.Information($"Frameworks                {string.Join(",", lib.Frameworks)}");
        }


        /// <summary>
        /// Performs set-up actions
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetLibSetup(this ICakeContext context, DotNetLib lib)
        {
            context.EnsureDirectoryExists(lib.PublishDirectory);
            context.EnsureDirectoryExists(lib.NuGetDirectory);
        }


        /// <summary>
        /// Cleans temporary files
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetLibClean(this ICakeContext context, DotNetLib lib)
        {
            context.CleanDirectories($"{lib.SolutionDirectory}/**/bin/{Build.Configuration}");
            context.CleanDirectories($"{lib.SolutionDirectory}/**/obj/{Build.Configuration}");
            context.CleanDirectory(lib.PublishDirectory);
            context.CleanDirectory(lib.NuGetDirectory);
        }


        /// <summary>
        /// Restores external packages and builds the project
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        /// <param name="settings">Restore build settings</param>
        [CakeMethodAlias]
        public static void DotNetLibBuild(
            this ICakeContext context,
            DotNetLib lib,
            DotNetCoreRestoreBuildSettings settings)
        {
            context.DotNetCoreRestore(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                settings.RestoreSettings);

            context.DotNetCoreBuild(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                settings.BuildSettings);
        }


        /// <summary>
        /// Publishes the project
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        /// <param name="settings">Restore publish settings</param>
        [CakeMethodAlias]
        public static void DotNetLibPublish(
            this ICakeContext context,
            DotNetLib lib,
            DotNetCoreRestorePublishSettings settings)
        {
            context.Information($"Publishing: {settings.PublishSettings.Framework}");

            context.DotNetCoreRestore(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                settings.RestoreSettings);

            context.DotNetCorePublish(
                lib.ProjectFile.FullPath,
                settings.PublishSettings);
        }


        /// <summary>
        /// Creates a nuget package from the publish files
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetLibPackage(this ICakeContext context, DotNetLib lib)
        {
            string CleanFrameworkName(string framework)
            {
                var index = framework.IndexOf('-');
                return index != -1 ? framework.Substring(0, index) : framework;
            }

            lib.NuGetFiles.AddRange(
                context.GetFiles($"{lib.PublishDirectory}/**/{lib.Project}.dll")
                    .Union(context.GetFiles($"{lib.PublishDirectory}/**/{lib.Project}.pdb"))
                    .Where(f => !f.FullPath.Contains("/runtimes/"))
                    .Select(f => f.FullPath.Substring(lib.PublishDirectory.FullPath.Length + 1))
                    .Select(f => new NuSpecContent { Source = f, Target = $"lib/{CleanFrameworkName(f)}" }));

            switch (Build.Verbosity)
            {
                case Verbosity.Diagnostic:
                case Verbosity.Normal:
                case Verbosity.Verbose:
                {
                    foreach (var file in lib.NuGetFiles)
                        context.Information($"Source: {file.Source}\tTarget: {file.Target}");
                } break;
            }

            var settings = lib.NuGetPackSettings;

            settings.Repository = new NuGetRepository
            {
                Branch = context.GitBranchCurrent(Build.GitRoot).CanonicalName,
                Commit = context.GitBranchCurrent(Build.GitRoot).Tip.Sha,
                Type = "git",
                Url = lib.ProjectUrl.ToString()
            };

            context.NuGetPack(settings);
            context.Information($"Packed: {lib.NuGetDirectory}/{lib.Id}.{lib.Version}.nupkg");
        }


        /// <summary>
        /// Pushes the nuget package to the remote repository
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetLibPush(this ICakeContext context, DotNetLib lib)
        {
            var pushSettings = lib.NuGetPushSettings;

            if (Build.Local)
            {
                var hasPacakge = context.NuGetList(
                    lib.Id,
                    new NuGetListSettings {
                        Source = new[] { pushSettings.Source },
                    })
                    .Any(i => i.Version == lib.Version.ToString());

                if (hasPacakge)
                    context.DotNetCoreNuGetDelete(
                        lib.Id,
                        lib.Version.ToString(),
                        new DotNetCoreNuGetDeleteSettings {
                            Source = pushSettings.Source,
                            NonInteractive = true
                        });
            }

            var file = context.File($"{lib.NuGetDirectory}/{lib.Id}.{lib.Version}.nupkg");

            context.NuGetPush(file.Path, pushSettings);
        }


        /// <summary>
        /// Dynamically determines the clean-up tasks dependent task
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        /// <returns>Task builder of the penultimate task</returns>
        [CakeMethodAlias]
        public static string DotNetLibCleanUpDependency(
            this ICakeContext context,
            DotNetLib lib)
        {
            switch (Build.Target)
            {
                case "InfoOnly":
                case "Build-InfoOnly":
                    return lib.Tasks.Names.Info;

                case "BuildAll":
                case "Build-BuildAll":
                    return lib.UsePostBuildTask
                        ? lib.Tasks.Names.PostBuild
                        : lib.Tasks.Names.Build;

                case "TestAll":
                case "Build-TestAll":
                    return lib.Tasks.Names.Test;

                case "PackageAll":
                case "Build-PackageAll":
                    return lib.Tasks.Names.Package;

                case "RunAll":
                case "Build-RunAll":
                default:
                    return lib.Tasks.Names.Push;

            }
        }


        /// <summary>
        /// Dynamically determines the clean-up tasks build dependee task
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        /// <returns>Task builder for the run target</returns>
        [CakeMethodAlias]
        public static string DotNetLibCleanUpDependee(
            this ICakeContext context,
            DotNetLib lib)
        {
            switch (Build.Target)
            {
                case "InfoOnly":
                case "Build-InfoOnly":
                    return Build.Targets.InfoOnly.Task.Name;

                case "BuildAll":
                case "Build-BuildAll":
                    return Build.Targets.BuildAll.Task.Name;

                case "TestAll":
                case "Build-TestAll":
                    return Build.Targets.TestAll.Task.Name;

                case "PackageAll":
                case "Build-PackageAll":
                    return Build.Targets.PackageAll.Task.Name;

                case "RunAll":
                case "Build-RunAll":
                default:
                    return Build.EnableCommits && Build.EnablePush
                        ? Build.Tasks.Push.Task.Name
                        : Build.Targets.RunAll.Task.Name;
            }
        }


        /// <summary>
        /// Configures the dependency, criteria and error handling for a dotnet-lib recipe task
        /// </summary>
        /// <param name="builder">Cake task builder</param>
        /// <param name="lib"><see cref="DotNetLib"/> recipe configuration</param>
        /// <param name="dependentOn">Dependent task builder</param>
        /// <param name="dependee">Dependee task builder</param>
        /// <returns></returns>
        public static CakeTaskBuilder ConfigTaskFor(
            this CakeTaskBuilder builder,
            DotNetLib lib,
            CakeTaskBuilder dependentOn,
            CakeTaskBuilder dependee)
        {
            if (dependentOn is null)
                throw new ArgumentNullException("Dependent task is null");

            if (dependee is null)
                throw new ArgumentNullException("Dependee task is null");

            builder.ConfigTaskFor(lib, dependentOn.Task.Name, dependee.Task.Name);
            return builder;
        }


        /// <summary>
        /// Configures the dependency, criteria and error handling for a dotnet-lib recipe task
        /// </summary>
        /// <param name="builder">Cake task builder</param>
        /// <param name="lib"><see cref="DotNetLib"/> recipe configuration</param>
        /// <param name="dependentOn">Dependent task name</param>
        /// <param name="dependee">Dependee task name</param>
        /// <returns></returns>
        public static CakeTaskBuilder ConfigTaskFor(
            this CakeTaskBuilder builder,
            DotNetLib lib,
            string dependentOn,
            string dependee = null)
        {
            // Bump dependent task forward based on recipe config
            if (dependentOn == lib.Tasks.Names.Commit && !lib.UseCommitTask)
                dependentOn = lib.Tasks.Names.Package;

            if (dependentOn == lib.Tasks.Names.PostBuild && !lib.UsePostBuildTask)
                dependentOn = lib.Tasks.Names.Build;

            if (dependentOn == lib.Tasks.Names.PreBuild && !lib.UsePreBuildTask)
                dependentOn = lib.Tasks.Names.Clean;

            if (dependentOn == lib.Tasks.Names.AssemblyInfo && !lib.UpdateAssemblyInfo)
                dependentOn = lib.Tasks.Names.ReleaseNotes;

            if (dependentOn == lib.Tasks.Names.ReleaseNotes && !lib.PrepareReleaseNotes)
                dependentOn = lib.Tasks.Names.Version;

            builder
                .IsDependentOn(dependentOn)
                .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
                .OnError(ex => lib.SetError(builder, ex));

            // Bump dependee back based on lib config
            if (!string.IsNullOrEmpty(dependee))
            {
                if (dependee == lib.Tasks.Names.ReleaseNotes && !lib.PrepareReleaseNotes)
                    dependee = lib.Tasks.Names.AssemblyInfo;

                if (dependee == lib.Tasks.Names.AssemblyInfo && !lib.UpdateAssemblyInfo)
                    dependee = lib.Tasks.Names.Clean;

                if (dependee == lib.Tasks.Names.PreBuild && !lib.UsePreBuildTask)
                    dependee = lib.Tasks.Names.Build;

                if (dependee == lib.Tasks.Names.PostBuild && !lib.UsePostBuildTask)
                    dependee = lib.Tasks.Names.Test;

                if (dependee == lib.Tasks.Names.Commit && !lib.UseCommitTask)
                    dependee = lib.Tasks.Names.Push;

                builder.IsDependeeOf(dependee);
            }

            return builder;
        }
    }
}
