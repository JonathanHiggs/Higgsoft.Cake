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
    var names = tasks.Names;

    tasks.Info = Task(names.Info)
        .ConfigTaskFor(app, Build.Tasks.Check.Task.Name)
        .Does(() => DotNetAppInfo(app));

    tasks.Setup = Task(names.Setup)
        .ConfigTaskFor(app, names.Info)
        .Does(() => DotNetAppSetup(app));

    tasks.Check = Task(names.Check)
        .ConfigTaskFor(app, names.Setup)
        .Does(() => RecipeCheck(app));

    tasks.Version = Task(names.Version)
        .ConfigTaskFor(app, names.Check)
        .Does(() => RecipeVersion(app));

    tasks.ReleaseNotes = Task(names.ReleaseNotes)
        .ConfigTaskFor(app, names.Version)
        .Does(() => RecipeReleaseNotes(app));

    if (app.UpdateAssemblyInfo)
        tasks.AssemblyInfo = Task(names.AssemblyInfo)
            .ConfigTaskFor(app, names.ReleaseNotes)
            .Does(() => RecipeAssemblyInfo(app));

    tasks.Clean = Task(names.Clean)
        .ConfigTaskFor(app, names.AssemblyInfo)
        .Does(() => DotNetAppClean(app));

    if (app.UsePreBuildTask)
        tasks.PreBuild = Task(names.PreBuild)
            .ConfigTaskFor(app, names.Clean);

    tasks.Build = Task(names.Build)
        .ConfigTaskFor(app, names.PreBuild)
        .DoesForEach(app.RestoreBuildSettings, settings => DotNetAppRestoreBuild(app, settings));

    if (app.UsePostBuildTask)
        tasks.PostBuild = Task(names.PostBuild)
            .ConfigTaskFor(app, names.Build);

    tasks.Test = Task(names.Test)
        .ConfigTaskFor(app, names.PostBuild)
        .Does(() => RecipeTest(app));

    tasks.Publish = Task(names.Publish)
        .ConfigTaskFor(app, names.Test)
        .DoesForEach(app.RestorePublishSettings, settings => DotNetAppRestorePublish(app, settings));

    // ToDo: swap order of package and commit
    tasks.Package = Task(names.Package)
        .ConfigTaskFor(app, names.Publish)
        .Does(() => DotNetAppPackage(app));

    if (app.UseCommitTask)
        tasks.Commit = Task(names.Commit)
            .ConfigTaskFor(app, names.Package)
            .Does(() => RecipeCommit(app));

    tasks.Push = Task(names.Push)
        .ConfigTaskFor(app, names.Commit)
        .Does(() => DotNetAppPush(app));
        
    tasks.CleanUp = Task(names.CleanUp)
        .IsDependentOn(DotNetAppCleanUpDependency(app))
        .IsDependeeOf(DotNetAppCleanUpDependee(app))
        .Does(() => RecipeCleanUp(app))
        .OnError(ex => app.SetError(ex));
};


Func<Action<DotNetApp>, DotNetApp> AddDotNetApp = (Action<DotNetApp> config) => {
    var app = ConfigDotNetApp(config);
    SetDotNetAppTasks(app);
    return app;
};