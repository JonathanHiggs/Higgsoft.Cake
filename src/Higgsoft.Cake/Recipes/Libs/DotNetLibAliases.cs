using System;
using System.Linq;
using System.Runtime.CompilerServices;

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
        /// Performs setup actions
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
            lib.NuGetFiles.AddRange(
                context.GetFiles($"{lib.PublishDirectory}/**/{lib.Project}.dll")
                    .Union(context.GetFiles($"{lib.PublishDirectory}/**/{lib.Project}.pdb"))
                    .Where(f => !f.FullPath.Contains("/runtimes/"))
                    .Select(f => f.FullPath.Substring(lib.PublishDirectory.FullPath.Length + 1))
                    .Select(f => new NuSpecContent { Source = f, Target = $"lib/{f} " }));

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

            context.NuGetPack(lib.NuGetPackSettings);
            context.Information($"Packed: {lib.NuGetDirectory}/{lib.Id}.{lib.Version}.nupkg");
        }


        /// <summary>
        /// Pushes the nuget package to the remote repo
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
        public static CakeTaskBuilder DotNetLibCleanUpDependency(
            this ICakeContext context,
            DotNetLib lib)
        {
            switch (Build.Target)
            {
                case "InfoOnly":
                case "Build-InfoOnly":
                    return lib.Tasks.Info;

                case "BuildAll":
                case "Build-BuildAll":
                    return lib.UsePostBuildTask ? lib.Tasks.PostBuild : lib.Tasks.Build;

                case "TestAll":
                case "Build-TestAll":
                    return lib.Tasks.Test;

                case "PackageAll":
                case "Build-PackageAll":
                    return lib.Tasks.Package;

                case "RunAll":
                case "Build-RunAll":
                default:
                    return lib.Tasks.Push;

            }
        }


        /// <summary>
        /// Dynamically determines the clean-up tasks build dependee task
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="lib">Recipe configuration</param>
        /// <returns>Task builder for the run target</returns>
        [CakeMethodAlias]
        public static CakeTaskBuilder DotNetLibCleanUpDependee(
            this ICakeContext context,
            DotNetLib lib)
        {
            switch (Build.Target)
            {
                case "InfoOnly":
                case "Build-InfoOnly":
                    return Build.Targets.InfoOnly;

                case "BuildAll":
                case "Build-BuildAll":
                    return Build.Targets.BuildAll;

                case "TestAll":
                case "Build-TestAll":
                    return Build.Targets.TestAll;

                case "PackageAll":
                case "Build-PackageAll":
                    return Build.Targets.PackageAll;

                case "RunAll":
                case "Build-RunAll":
                default:
                    return Build.EnableCommits && Build.EnablePush
                        ? Build.Tasks.Push
                        : Build.Targets.RunAll;
            }
        }


        /// <summary>
        /// Configures the depencency, criteria and error handling for a dotnet-lib task
        /// </summary>
        /// <param name="builder">Cake task builder</param>
        /// <param name="lib"><see cref="DotNetLib"/> configuration</param>
        /// <param name="prev">Depencent task builder</param>
        /// <param name="before">Dependee task builder</param>
        /// <returns></returns>
        public static CakeTaskBuilder ConfigTaskFor(
            this CakeTaskBuilder builder,
            DotNetLib lib,
            CakeTaskBuilder prev,
            CakeTaskBuilder before = null)
        {
            // Bump depencency forward based on lib config
            if (prev == lib.Tasks.Commit && !lib.UseCommitTask)
                prev = lib.Tasks.Package;

            if (prev == lib.Tasks.PostBuild && !lib.UsePostBuildTask)
                prev = lib.Tasks.Build;

            if (prev == lib.Tasks.PreBuild && !lib.UsePreBuildTask)
                prev = lib.Tasks.Clean;

            if (prev == lib.Tasks.AssemblyInfo && !lib.UpdateAssemblyInfo)
                prev = lib.Tasks.ReleaseNotes;

            if (prev == lib.Tasks.ReleaseNotes && !lib.PrepareReleaseNotes)
                prev = lib.Tasks.Version;

            builder
                .IsDependentOn(prev)
                .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
                .OnError(ex => { lib.SetError(builder, ex); });

            // Bump dependee back based on lib config
            if (!(before is null))
            {
                if (before == lib.Tasks.ReleaseNotes && !lib.PrepareReleaseNotes)
                    before = lib.Tasks.AssemblyInfo;

                if (before == lib.Tasks.AssemblyInfo && !lib.UpdateAssemblyInfo)
                    before = lib.Tasks.Clean;

                if (before == lib.Tasks.PreBuild && !lib.UsePreBuildTask)
                    before = lib.Tasks.Build;

                if (before == lib.Tasks.PostBuild && !lib.UsePostBuildTask)
                    before = lib.Tasks.Test;

                if (before == lib.Tasks.Commit && !lib.UseCommitTask)
                    before = lib.Tasks.Push;

                builder.IsDependeeOf(before.Task.Name);
            }

            return builder;
        }
    }
}
