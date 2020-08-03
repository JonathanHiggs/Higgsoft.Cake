//////////////////////
// Includes & Using
////////////////////

#tool nuget:?package=NUnit.ConsoleRunner&version=3.9.0

#addin nuget:?package=Higgsoft.Cake&version=0.1.0

using Higgsoft.Cake.Recipes;
using Higgsoft.Cake.Recipes.Libs;


//////////////////////
// Task Factory
////////////////////

Func<Action<DotNetLib>, DotNetLib> ConfigDotNetLib = (Action<DotNetLib> config) => {
    var lib = new DotNetLib();
    config(lib);
    Build.RecipeBuilds.Add(lib);
    return lib;
};


Action<DotNetLib> SetDotNetLibTasks = (DotNetLib lib) => {
    var tasks = lib.Tasks;

    tasks.Info = Task($"{lib.Id}-Info")
        .IsDependentOn(Build.PreBuild)
        .IsDependeeOf(Build.InfoOnly.Task.Name)
        .Does(() => {
            Information("DotNet Lib:");
            Information($"Id                        {lib.Id}");
            Information($"Name                      {lib.Name}");
            Information($"Description               {lib.Description}");
            Information($"Solution                  {lib.Solution}");
            Information($"Project                   {lib.Project}");
            Information("\nPaths");
            Information($"Solution Directory        {lib.SolutionDirectory}");
            Information($"Solution File             {lib.SolutionFile}");
            Information($"Project File              {lib.ProjectFile}");
            Information($"Assembly Info File        {lib.AssemblyInfoFile}");
            Information($"Release Notes File        {lib.ReleaseNotesFile}");
            //Information($"Build Directory           {lib.BuildDirectory}");
            Information($"Publish Directory         {lib.PublishDirectory}");
            Information($"NuGet Directory           {lib.NuGetDirectory}");
            Information("\nSettings");
            Information($"Prepare Release Notes     {lib.PrepareReleaseNotes}");
            Information($"Update Assembly Info      {lib.UpdateAssemblyInfo}");
            Information($"Commit Changes            {lib.CommitChanges}");
            Information($"Tag Version               {lib.TagVersion}");
            Information($"Push To Remote            {lib.PushToRemote}");
            Information($"Remote Name               {lib.RemoteName}");
            Information($"Frameworks                {string.Join(",", lib.Frameworks)}");
        });

    tasks.Setup = Task($"{lib.Id}-Setup")
        .IsDependentOn(tasks.Info)
        .Does(() => {
            //EnsureDirectoryExists(lib.BuildDirectory);
            EnsureDirectoryExists(lib.PublishDirectory);
            EnsureDirectoryExists(lib.NuGetDirectory);
        })
        .OnError(ex => {
            lib.SetError(tasks.Setup, ex);
            Error(ex);
        });

    tasks.Check = Task($"{lib.Id}-Check")
        .IsDependentOn(tasks.Setup)
        .WithCriteria(() => !lib.Errored)
        .Does(() => {
            Check(lib.CheckSettings);

            lib.SkipRemainingTasks =
                lib.PrepareReleaseNotes && !ReleaseNotesUpdated(lib.ReleaseNotesSettings);
        })
        .OnError(ex => {
            lib.SetError(tasks.Check, ex);
            Error(ex);
        });

    tasks.Version = Task($"{lib.Id}-Version")
        .IsDependentOn(tasks.Check)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => lib.Version = ParseVersionFrom(lib.ReleaseNotesVNextFile))
        .OnError(ex => {
            lib.SetError(tasks.Version, ex);
            Error(ex);
        });

    tasks.ReleaseNotes = Task($"{lib.Id}-ReleaseNotes")
        .IsDependentOn(tasks.Version)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.PrepareReleaseNotes)
        .Does(() => lib.ReleaseNotes.AddRange(UpdateReleaseNotes(lib.ReleaseNotesSettings)))
        .OnError(ex => {
            lib.SetError(tasks.ReleaseNotes, ex);
            Error(ex);
        });

    tasks.AssemblyInfo = Task($"{lib.Id}-AssemblyInfo")
        .IsDependentOn(tasks.ReleaseNotes)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.UpdateAssemblyInfo)
        .Does(() => CreateAssemblyInfo(lib.AssemblyInfoFile, lib.AssemblyInfoSettings))
        .OnError(ex => {
            lib.SetError(tasks.AssemblyInfo, ex);
            Error(ex);
        });

    tasks.Clean = Task($"{lib.Id}-Clean")
        .IsDependentOn(tasks.AssemblyInfo)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => {
            CleanDirectories($"{lib.SolutionDirectory}/**/bin/{Build.Configuration}");
            CleanDirectories($"{lib.SolutionDirectory}/**/obj/{Build.Configuration}");
            //CleanDirectory(lib.BuildDirectory);
            CleanDirectory(lib.PublishDirectory);
            CleanDirectory(lib.NuGetDirectory);
        })
        .OnError(ex => {
            lib.SetError(tasks.Clean, ex);
            Error(ex);
        });

    tasks.Restore = Task($"{lib.Id}-Restore")
        .IsDependentOn(tasks.Clean)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() =>
            DotNetCoreRestore(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                lib.RestoreSettings))
        .OnError(ex => {
            lib.SetError(tasks.Restore, ex);
            Error(ex);
        });

    tasks.PreBuild = Task($"{lib.Id}-PreBuild")
        .IsDependentOn(tasks.Restore)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.UsePreBuildTask)
        .OnError(ex => {
            lib.SetError(tasks.PreBuild, ex);
            Error(ex);
        });

    tasks.Build = Task($"{lib.Id}-Build")
        .IsDependentOn(tasks.PreBuild)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() =>
            DotNetCoreBuild(
                lib.SolutionFile?.FullPath ?? lib.ProjectFile.FullPath,
                lib.BuildSettings))
        .OnError(ex => {
            lib.SetError(tasks.Build, ex);
            Error(ex);
        });

    tasks.PostBuild = Task($"{lib.Id}-PostBuild")
        .IsDependentOn(tasks.Build)
        .IsDependeeOf(Build.BuildAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.UsePostBuildTask)
        .OnError(ex => {
            lib.SetError(tasks.PostBuild, ex);
            Error(ex);
        });

    tasks.Test = Task($"{lib.Id}-Test")
        .IsDependentOn(tasks.PostBuild)
        .IsDependeeOf(Build.TestAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => NUnit3($"./**/bin/{Build.Configuration}/*.UnitTests.dll", lib.NUnitSettings))
        .OnError(ex => {
            lib.SetError(tasks.Test, ex);
            Error(ex);
        });

    tasks.Publish = Task($"{lib.Id}-Publish")
        .IsDependentOn(tasks.Test)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .DoesForEach(
            lib.PublishSettings,
            settings => {
                Information(settings.Framework);
                DotNetCorePublish(lib.ProjectFile.FullPath, settings);
        })
        .OnError(ex => {
            lib.SetError(tasks.Publish, ex);
            Error(ex);
        });

    tasks.Package = Task($"{lib.Id}-Package")
        .IsDependentOn(tasks.Publish)
        .IsDependeeOf(Build.PackageAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => {
            lib.NuGetFiles.AddRange(
                GetFiles($"{lib.PublishDirectory}/**/{lib.Project}.dll")
                    .Union(GetFiles($"{lib.PublishDirectory}/**/{lib.Project}.pdb"))
                    .Where(f => f.FullPath.Contains($"{lib.Project}.") && !f.FullPath.Contains("/runtimes/"))
                    .Select(f => f.FullPath.Substring(lib.PublishDirectory.FullPath.Length + 1))
                    .Select(f => new NuSpecContent { Source = f, Target = $"lib/{f}" }));

            foreach (var file in lib.NuGetFiles)
                Information(file.Source);

            NuGetPack(lib.NuGetPackSettings);
        })
        .OnError(ex => {
            lib.SetError(tasks.Package, ex);
            Error(ex);
        });

    tasks.Commit = Task($"{lib.Id}-Commit")
        .IsDependentOn(tasks.Package)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && !Build.Local)
        .Does(() => CommitChanges(lib.CommitSettings))
        .OnError(ex => {
            lib.SetError(tasks.Commit, ex);
            Error(ex);
        });

    tasks.Push = Task($"{lib.Id}-Push")
        .IsDependentOn(tasks.Commit)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() =>
            NuGetPush(
                File(lib.NuGetDirectory + $"/{lib.Id}.{lib.Version}.nupkg"),
                lib.NuGetPushSettings))
        .OnError(ex => {
            lib.SetError(tasks.Push, ex);
            Error(ex);
        });

    tasks.CleanUp = Task($"{lib.Id}-CleanUp")
        .IsDependentOn(tasks.Push)
        .IsDependeeOf(Build.RunAll.Task.Name)
        .WithCriteria(() => lib.Errored || lib.SkipRemainingTasks || Build.Local)
        .Does(() => RevertChanges(lib.RevertSettings))
        .OnError(ex => {
            lib.SetError(tasks.Commit, ex);
            Error(ex);
        });
};


Func<Action<DotNetLib>, DotNetLib> AddDotNetLib = (Action<DotNetLib> config) => {
    var lib = ConfigDotNetLib(config);
    SetDotNetLibTasks(lib);
    return lib;
};