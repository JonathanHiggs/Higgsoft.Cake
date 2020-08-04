﻿using System;

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
            // ToDo: check for adding recipes twice
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
            context.Information($"Artefacts Directory       {app.ArtefactDirectory}");
            context.Information("\nSettings");
            context.Information($"Prepare Release Notes     {app.PrepareReleaseNotes}");
            context.Information($"Update Assembly Info      {app.UpdateAssemblyInfo}");
            context.Information($"Commit Changes            {app.CommitChanges}");
            context.Information($"Tag Version               {app.TagVersion}");
            context.Information($"Push To Remote            {app.PushToRemote}");
            context.Information($"Remote Name               {app.RemoteName}");
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
            context.EnsureDirectoryExists(app.ArtefactDirectory);
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
        /// <param name="settings"></param>
        [CakeMethodAlias]
        public static void DotNetAppRestoreBuild(
            this ICakeContext context,
            DotNetApp app,
            DotNetApp.DotNetCoreRestoreBuildSettings settings)
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
        /// <param name="settings"></param>
        [CakeMethodAlias]
        public static void DotNetAppRestorePublish(
            this ICakeContext context,
            DotNetApp app,
            DotNetApp.DotNetCoreRestorePublishSettings settings)
        {
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
            => context.CopyFile(app.TempArtefactFile, app.ArtefactFile);
    }
}
