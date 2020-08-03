//////////////////////
// Includes & Using
////////////////////

#addin nuget:?package=Higgsoft.Cake&version=0.1.0
#addin nuget:?package=Cake.Git

using Higgsoft.Cake.Recipes;


//////////////////////
// Arguments
////////////////////

Build.Target                    = Argument("target",                Build.Target);
Build.Configuration             = Argument("configuration",         Build.Configuration);
Build.Verbosity                 = Argument("verbosity",             Build.Verbosity);
Build.Local                     = Argument("local",                 Build.Local);

Build.CheckStagedChanges        = Argument("check-staged",          Build.CheckStaged);
Build.CheckUncommittedChanges   = Argument("check-uncommitted",     Build.CheckUncommittedChanges);
Build.CheckUntrackedFiles       = Argument("check-untracked",       Build.CheckUntrackedFiles);

Build.GitRoot                   = GitFindRootFromPath(MakeAbsolute(Directory(".")));
Build.EnableCommits             = Argument("enable-commits",        Build.EnableCommits);
Build.EnableTags                = Argument("enable-tags",           Build.EnableTags);

Build.NuGetSource               = Argument("nuget-source",          Build.NuGetSource);
Build.NuGetLocalSource          = Argument("nuget-local-source",    Build.NuGetLocalSource);
Build.NuGetApiKey               = Argument("nuget-api-key",         EnvironmentVariable("NUGET_API_KEY"));

Build.SquirrelCentralRepository = Argument("squirrel-repo",         Build.SquirrelCentralRepository);
Build.SquirrelLocalRepository   = Argument("squirrel-local-repo",   Build.SquirrelLocalRepository);


//////////////////////
// Tasks
////////////////////

Build.Info = Task("Info")
    .Does(() => BuildInfo());

Build.PreBuild = Task("PreBuild")
    .IsDependentOn("Info")
    .Does(() => Check(Build.CheckSettings));


//////////////////////
// Targets
////////////////////

Build.InfoOnly = Task("InfoOnly")
    .IsDependentOn("Info")
    .Does(() => BuildStatus());

Build.BuildAll = Task("BuildAll")
    .Does(() => BuildStatus());

Build.TestAll = Task("TestAll")
    .Does(() => BuildStatus());

Build.PackageAll = Task("PackageAll")
    .Does(() => BuildStatus());

Build.RunAll = Task("RunAll")
    .Does(() => BuildStatus());