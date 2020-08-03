using System;

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Core;
using Cake.Core.Annotations;

namespace Higgsoft.Cake.Recipes.Apps
{
    [CakeAliasCategory("Higgsoft.Cake.Recipes.Apps")]
    public static class DotNetAppAliases
    {
        [CakeMethodAlias]
        public static DotNetApp ConfigDotNetApp(this ICakeContext context, Action<DotNetApp> config)
        {
            var app = new DotNetApp();
            config(app);
            Build.RecipeBuilds.Add(app);
            return app;
        }


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


        [CakeMethodAlias]
        public static void DotNetAppSetup(this ICakeContext context, DotNetApp app)
        {
            context.EnsureDirectoryExists(app.PublishDirectory);
            context.EnsureDirectoryExists(app.PackageDirectory);
        }


        [CakeMethodAlias]
        public static void DotNetAppClean(this ICakeContext context, DotNetApp app)
        {
            context.CleanDirectories($"{app.SolutionDirectory}/**/bin/{Build.Configuration}");
            context.CleanDirectories($"{app.SolutionDirectory}/**/obj/{Build.Configuration}");
            context.CleanDirectory(app.PublishDirectory);
            context.CleanDirectory(app.PackageDirectory);
        }


        [CakeMethodAlias]
        public static void DotNetAppRestoreBuild(
            this ICakeContext context,
            DotNetApp app,
            (DotNetCoreRestoreSettings Restore, DotNetCoreBuildSettings Build) settings)
        {
            context.DotNetCoreRestore(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.Restore);

            context.DotNetCoreBuild(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.Build);
        }


        [CakeMethodAlias]
        public static void DotNetAppRestorePublish(
            this ICakeContext context,
            DotNetApp app,
            (DotNetCoreRestoreSettings Restore, DotNetCorePublishSettings Publish) settings)
        {
            context.DotNetCoreRestore(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.Restore);

            context.DotNetCorePublish(
                app.SolutionFile?.FullPath ?? app.ProjectFile.FullPath,
                settings.Publish);
        }


        [CakeMethodAlias]
        public static void DotNetAppPackage(this ICakeContext context, DotNetApp app)
        {
            context.Information("Package...");
        }


        [CakeMethodAlias]
        public static void DotNetAppPush(this ICakeContext context, DotNetApp app)
        {
            context.Information("Push...");
        }
    }
}
