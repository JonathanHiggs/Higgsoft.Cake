//////////////////////
// Includes & Using
////////////////////

#tool nuget:?package=NUnit.ConsoleRunner&version=3.9.0

#addin nuget:?package=Higgsoft.Cake&version=0.1.0

using Higgsoft.Cake.Recipes;
using Higgsoft.Cake.Recipes.Apps;


//////////////////////
// Task Factory
////////////////////

Action<DotNetApp> SetDotNetAppTasks = (DotNetApp app) => {
    var tasks = app.Tasks;

    tasks.Info = Task($"{app.Id}-Info")
        .IsDependentOn(Build.PreBuild)
        .IsDependeeOf(Build.InfoOnly.Task.Name)
        .Does(() => DotNetAppInfo(app));

    tasks.Setup = Task($"{app.Id}-Setup")
        .IsDependentOn(tasks.Info)
        .Does(() => DotNetAppSetup(app))
        .OnError(ex => RecipeOnError(app, tasks.Setup, ex));

    tasks.Check = Task($"{app.Id}-Check")
        .IsDependentOn(tasks.Setup)
        .WithCriteria(() => !app.Errored)
        .Does(() => RecipeCheck(app))
        .OnError(ex => RecipeOnError(app, tasks.Check, ex));

    tasks.Version = Task($"{app.Id}-Version")
        .IsDependentOn(tasks.Check)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .Does(() => RecipeVersion(app))
        .OnError(ex => RecipeOnError(app, tasks.Version, ex));

    tasks.ReleaseNotes = Task($"{app.Id}-ReleaseNotes")
        .IsDependentOn(tasks.Version)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored && app.PrepareReleaseNotes)
        .Does(() => RecipeReleaseNotes(app))
        .OnError(ex => RecipeOnError(app, tasks.ReleaseNotes, ex));

    if (app.UpdateAssemblyInfo)
        tasks.AssemblyInfo = Task($"{app.Id}-AssemblyInfo")
            .IsDependentOn(tasks.ReleaseNotes)
            .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
            .Does(() => RecipeAssemblyInfo(app))
            .OnError(ex => RecipeOnError(app, tasks.AssemblyInfo, ex));

    tasks.Clean = Task($"{app.Id}-Clean")
        .IsDependentOn(app.UpdateAssemblyInfo ? tasks.AssemblyInfo : tasks.ReleaseNotes)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .Does(() => DotNetAppClean(app))
        .OnError(ex => RecipeOnError(app, tasks.Clean, ex));

    if (app.UsePreBuildTask)
        tasks.PreBuild = Task($"{app.Id}-PreBuild")
            .IsDependentOn(tasks.Clean)
            .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
            .OnError(ex => RecipeOnError(app, tasks.PreBuild, ex));

    tasks.Build = Task($"{app.Id}-Build")
        .IsDependentOn(app.UsePreBuildTask ? tasks.PreBuild : tasks.Clean)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .DoesForEach(app.RestoreBuildSettings, settings => DotNetAppRestoreBuild(app, settings))
        .OnError(ex => RecipeOnError(app, tasks.Build, ex));

    if (app.UsePostBuildTask)
        tasks.PostBuild = Task($"{app.Id}-PostBuild")
            .IsDependentOn(tasks.Build)
            .IsDependeeOf(Build.BuildAll.Task.Name)
            .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
            .OnError(ex => RecipeOnError(app, tasks.PostBuild, ex));

    tasks.Test = Task($"{app.Id}-Test")
        .IsDependentOn(app.UsePostBuildTask ? tasks.PostBuild : tasks.Build)
        .IsDependeeOf(Build.TestAll.Task.Name)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .Does(() => RecipeTest(app))
        .OnError(ex => RecipeOnError(app, tasks.Test, ex));

    tasks.Publish = Task($"{app.Id}-Publish")
        .IsDependentOn(tasks.Test)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .DoesForEach(app.RestorePublishSettings, settings => DotNetAppRestorePublish(app, settings))
        .OnError(ex => RecipeOnError(app, tasks.Publish, ex));

    tasks.Package = Task($"{app.Id}-Package")
        .IsDependentOn(tasks.Publish)
        .IsDependeeOf(Build.PackageAll.Task.Name)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .Does(() => DotNetAppPackage(app))
        .OnError(ex => RecipeOnError(app, tasks.Package, ex));

    if (!Build.Local)
        tasks.Commit = Task($"{app.Id}-Commit")
            .IsDependentOn(tasks.Package)
            .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
            .Does(() => RecipeCommit(app))
            .OnError(ex => RecipeOnError(app, tasks.Commit, ex));

    tasks.Push = Task($"{app.Id}-Push")
        .IsDependentOn(!Build.Local ? tasks.Commit : tasks.Package)
        .WithCriteria(() => !app.SkipRemainingTasks && !app.Errored)
        .Does(() => DotNetAppPush(app))
        .OnError(ex => RecipeOnError(app, tasks.Push, ex));

    tasks.CleanUp = Task($"{app.Id}-CleanUp")
        .IsDependentOn(tasks.Push)
        .IsDependeeOf(Build.RunAll.Task.Name)
        .Does(() => RecipeCleanUp(app))
        .OnError(ex => RecipeOnError(app, tasks.CleanUp, ex));
};


Func<Action<DotNetApp>, DotNetApp> AddDotNetApp = (Action<DotNetApp> config) => {
    var app = ConfigDotNetApp(config);
    SetDotNetAppTasks(app);
    return app;
};