using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.Annotations;

namespace Higgsoft.Cake.Recipes.Apps
{
    /// <summary>
    /// Extension methods for <see cref="DotNetApp"/> recipes
    /// </summary>
    [CakeAliasCategory("Higgsoft.Cake.Recipes.Apps")]
    public static class DotNetAppAliases
    {
        /// <summary>
        /// Configures a <see cref="DotNetApp"/> recipe from a configuration action
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="config">Configuration action</param>
        /// <returns><see cref="DotNetApp"/> recipe settings</returns>
        [CakeMethodAlias]
        public static DotNetApp ConfigDotNetApp(this ICakeContext context, Action<DotNetApp> config)
        {
            var app = new DotNetApp();
            config(app);

            if (Build.RecipeBuilds.Any(r => r.Id == app.Id))
                throw new InvalidOperationException(
                    $"Recipe with id: {app.Id} already added to the build");

            Build.RecipeBuilds.Add(app);
            return app;
        }


        /// <summary>
        /// Logs the recipe settings to the build output
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetAppInfo(this ICakeContext context, DotNetApp app)
        {
            context.Information("DotNet App:");
            context.Information($"Id                        {app.Id}");
            context.Information($"Name                      {app.Name}");
            context.Information($"Description               {app.Description}");
            context.Information($"Solution                  {app.Solution}");
            context.Information($"Project                   {app.Project}");
            context.Information("\nPaths");
            context.Information($"Solution Directory        {app.SolutionDirectory}");
            context.Information($"Solution File             {app.SolutionFile}");
            context.Information($"Project File              {app.ProjectFile}");
            context.Information($"Assembly Info File        {app.AssemblyInfoFile}");
            context.Information($"Release Notes File        {app.ReleaseNotesFile}");
            context.Information($"Publish Directory         {app.PublishDirectory}");
            context.Information($"Package Directory         {app.PackageDirectory}");
            context.Information($"Artefacts Repository      {app.ArtefactsRepository}");
            context.Information("\nSettings");
            context.Information($"Prepare Release Notes     {app.PrepareReleaseNotes}");
            context.Information($"Update Assembly Info      {app.UpdateAssemblyInfo}");
            context.Information($"Commit Changes            {app.CommitChanges}");
            context.Information($"Tag Version               {app.TagVersion}");
            context.Information($"Runtimes                  {string.Join(",", app.Runtimes)}");
            context.Information($"Frameworks                {string.Join(",", app.Frameworks)}");
        }


        /// <summary>
        /// Performs setup actions
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetAppSetup(this ICakeContext context, DotNetApp app)
        {
            context.EnsureDirectoryExists(app.PublishDirectory);
            context.EnsureDirectoryExists(app.PackageDirectory);
            context.EnsureDirectoryExists(app.ArtefactsRepository);
        }


        /// <summary>
        /// Cleans temporary files
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetAppClean(this ICakeContext context, DotNetApp app)
        {
            context.CleanDirectories($"{app.SolutionDirectory}/**/bin/{Build.Configuration}");
            context.CleanDirectories($"{app.SolutionDirectory}/**/obj/{Build.Configuration}");
            context.CleanDirectory(app.PublishDirectory);
            context.CleanDirectory(app.PackageDirectory);
        }


        /// <summary>
        /// Restores external packages and builds the project
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        /// <param name="settings">Restore build settings</param>
        [CakeMethodAlias]
        public static void DotNetAppRestoreBuild(
            this ICakeContext context,
            DotNetApp app,
            DotNetCoreRestoreBuildSettings settings)
        {
            context.DotNetCoreRestore(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.RestoreSettings);

            context.DotNetCoreBuild(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.BuildSettings);
        }


        /// <summary>
        /// Restores external packages and publishes the project
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        /// <param name="settings">Restore publish settings</param>
        [CakeMethodAlias]
        public static void DotNetAppRestorePublish(
            this ICakeContext context,
            DotNetApp app,
            DotNetCoreRestorePublishSettings settings)
        {
            context.Information(
                $"Publishing: {settings.PublishSettings.Framework} {settings.PublishSettings.Runtime}");

            context.DotNetCoreRestore(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.RestoreSettings);

            context.DotNetCorePublish(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.PublishSettings);
        }


        /// <summary>
        /// Creates a build artefact from the published files
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetAppPackage(this ICakeContext context, DotNetApp app)
            => context.Zip(app.PublishDirectory, app.TempArtefactFile);


        /// <summary>
        /// Copies the build artefact to archive directory
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        [CakeMethodAlias]
        public static void DotNetAppPush(this ICakeContext context, DotNetApp app)
        {
            // ToDo: delete existing file if local build

            context.CopyFile(app.TempArtefactFile, app.ArtefactFile);
        }


        /// <summary>
        /// Dynamically determines the clean-up tasks dependent task
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        /// <returns>Task builder of the penultimate task</returns>
        [CakeMethodAlias]
        public static CakeTaskBuilder DotNetAppCleanUpDependency(
            this ICakeContext context,
            DotNetApp app)
        {
            switch (Build.Target)
            {
                case "InfoOnly":
                    return app.Tasks.Info;

                case "BuildAll":
                    return app.UsePostBuildTask ? app.Tasks.PostBuild : app.Tasks.Build;

                case "TestAll":
                    return app.Tasks.Test;

                case "PackageAll":
                    return app.Tasks.Package;

                case "RunAll":
                default:
                    return app.Tasks.Push;

            }
        }


        /// <summary>
        /// Dynamically determines the clean-up tasks build dependee task
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="app">Recipe configuration</param>
        /// <returns>Name of the build target task</returns>
        [CakeMethodAlias]
        public static string DotNetAppCleanUpDependee(
            this ICakeContext context,
            DotNetApp app)
        {
            switch (Build.Target)
            {
                case "InfoOnly":
                    return Build.Targets.InfoOnly.Task.Name;

                case "BuildAll":
                    return Build.Targets.BuildAll.Task.Name;

                case "TestAll":
                    return Build.Targets.TestAll.Task.Name;

                case "PackageAll":
                    return Build.Targets.PackageAll.Task.Name;

                case "RunAll":
                default:
                    return Build.EnableCommits && Build.EnablePush
                        ? Build.Tasks.Push.Task.Name
                        : Build.Targets.RunAll.Task.Name;
            }
        }
    }
}
