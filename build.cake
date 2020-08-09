//////////////////////////
// Includes & Using
////////////////////////

#addin nuget:?package=Cake.FileHelpers

#load nuget:?package=Higgsoft.Cake&version=0.1.0


//////////////////////////
// Arguments & Settings
////////////////////////

Build.GitEmail = "cake@higgsoft.com";


//////////////////////////
// Definitions
////////////////////////

var lib = AddDotNetLib(lib => {
    lib.Id                          = "Higgsoft.Cake";
    lib.Name                        = "Higgsoft.Cake";
    lib.Description                 = "Cake build tools and recipes for automated .Net builds";
    lib.Solution                    = "Higgsoft.Cake";
    lib.Project                     = "Higgsoft.Cake";
    
    lib.SolutionDirectory           = Directory($"{Build.GitRoot}/src");
    lib.SolutionFile                = File($"{lib.SolutionDirectory}/{lib.Solution}.sln");
    lib.ProjectFile                 = File($"{lib.SolutionDirectory}/{lib.Project}/{lib.Project}.csproj");
    lib.AssemblyInfoFile            = null;
    lib.ReleaseNotesFile            = File($"{Build.GitRoot}/ReleaseNotes.md");
    lib.ReleaseNotesVNextFile       = File($"{Build.GitRoot}/ReleaseNotes.vnext.md");
    lib.PublishDirectory            = Directory($"{Build.GitRoot}/publish");
    lib.NuGetDirectory              = Directory($"{Build.GitRoot}/nuget");

    lib.SharedAssemblyInfoFile      = false;
    lib.UsePreBuildTask             = false;
    lib.UsePostBuildTask            = false;
    lib.PrepareReleaseNotes         = true;
    lib.UpdateAssemblyInfo          = false;
    lib.CommitChanges               = true;
    lib.TagVersion                  = true;

    lib.ProjectUrl                  = new Uri("https://github.com/JonathanHiggs/Higgsoft.Cake");

    lib.AddFrameworks("net48", "netstandard2.0");
    lib.AddAuthors("Jonathan Higgs");

    lib.AddNuGetFiles(
        GetFiles("./src/**/*recipe.cake")
            .Select(f => MakeAbsolute(f))
            .Select(f => new NuSpecContent {
                Source = f.FullPath,
                Target = $"content/{f.GetFilename()}"
            }).ToArray());
});

var recipeTests = new[] {"dotnet-lib", "dotnet-app"};


//////////////////////////
// Tasks
////////////////////////

lib.Tasks.Version.Does(() => {
    UpdateScriptVersion("./**/*recipe.cake", lib.Id, lib.Version);
    UpdateScriptVersion("./test/**/build.cake", lib.Id, lib.Version);
});

Task("Higgsoft.Cake-AliasTests")
    .ConfigTaskFor(lib, lib.Tasks.Names.Package, lib.Tasks.Names.Commit)
    .Does(() => { });

Task("Higgsoft.Cake-RecipeTests")
    .ConfigTaskFor(lib, lib.Tasks.Names.Package, lib.Tasks.Names.Commit)
    .DoesForEach(recipeTests, recipe => {
        var dir = $"{Build.GitRoot}/test/{recipe}/tools";

        CakeInstallPackage(settings => {
            settings.Id = lib.Id;
            settings.Version = lib.Version.ToString();
            settings.NuGetDirectory = lib.NuGetDirectory;
            settings.ToolsDirectory = dir;
            settings.AsAddin = true;
            settings.AsTool = true; });

        CakeExecuteScript(
            $"{Build.GitRoot}/test/{recipe}/build.cake",
            new CakeSettings {
                Arguments = new Dictionary<string, string>{
                    { "local", "true" },
                    { "check-staged", "false" },
                    { "check-uncommitted", "false" },
                    { "enable-commits", "false" },
                    { "enable-push", "false" } },
                WorkingDirectory = Directory($"{Build.GitRoot}/test/{recipe}") });
    });


//////////////////////////
// Invoke
////////////////////////

RunTarget(Build.Target);
