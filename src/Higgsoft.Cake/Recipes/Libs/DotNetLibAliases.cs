using System;
using System.Linq;

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Solution.Project.Properties;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.NuGet.Delete;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Common.Tools.NUnit;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

using Higgsoft.Cake.Check;
using Higgsoft.Cake.Commit;
using Higgsoft.Cake.ReleaseNotes;
using Higgsoft.Cake.Versions;

namespace Higgsoft.Cake.Recipes.Libs
{
    [CakeAliasCategory("Higgsoft.Cake.Recipes.Libs")]
    public static class DotNetLibAliases
    {
        [CakeMethodAlias]
        public static DotNetLib ConfigDotNetLib(this ICakeContext context, Action<DotNetLib> config)
        {
            var lib = new DotNetLib();
            config(lib);
            Build.RecipeBuilds.Add(lib);
            return lib;
        }


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
            context.Information($"Push To Remote            {lib.PushToRemote}");
            context.Information($"Remote Name               {lib.RemoteName}");
            context.Information($"Frameworks                {string.Join(",", lib.Frameworks)}");
        }


        [CakeMethodAlias]
        public static void DotNetLibSetup(this ICakeContext context, DotNetLib lib)
        {
            context.EnsureDirectoryExists(lib.PublishDirectory);
            context.EnsureDirectoryExists(lib.NuGetDirectory);
        }


        [CakeMethodAlias]
        public static void DotNetLibClean(this ICakeContext context, DotNetLib lib)
        {
            context.CleanDirectories($"{lib.SolutionDirectory}/**/bin/{Build.Configuration}");
            context.CleanDirectories($"{lib.SolutionDirectory}/**/obj/{Build.Configuration}");
            context.CleanDirectory(lib.PublishDirectory);
            context.CleanDirectory(lib.NuGetDirectory);
        }


        [CakeMethodAlias]
        public static void DotNetLibRestore(this ICakeContext context, DotNetLib lib)
        {
            context.DotNetCoreRestore(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                lib.RestoreSettings);
        }


        [CakeMethodAlias]
        public static void DotNetLibBuild(this ICakeContext context, DotNetLib lib)
            // ToDo: build for each framework
            => context.DotNetCoreBuild(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                lib.BuildSettings);


        [CakeMethodAlias]
        public static void DotNetLibPublish(
            this ICakeContext context,
            DotNetLib lib,
            DotNetCorePublishSettings settings)
        {
            context.Information($"Publishing: {settings.Framework}");
            context.DotNetCorePublish(lib.ProjectFile.FullPath, settings);
        }


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
        }


        [CakeMethodAlias]
        public static void DotNetLibPush(this ICakeContext context, DotNetLib lib)
        {
            if (Build.Local)
            {
                context.DotNetCoreNuGetDelete(
                    lib.Id,
                    lib.Version.ToString(),
                    new DotNetCoreNuGetDeleteSettings {
                        Source = "Local",
                        NonInteractive = true
                    });
            }

            var file = context.File($"{lib.NuGetDirectory}/{lib.Id}.{lib.Version}.nupkg");

            context.NuGetPush(file.Path, lib.NuGetPushSettings);
        }
    }
}
