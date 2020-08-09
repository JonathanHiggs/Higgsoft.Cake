using System.Linq;

using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Git;

using Higgsoft.Cake.Commit;

namespace Higgsoft.Cake.Recipes
{
    [CakeAliasCategory("Higgsoft.Build")]
    public static class BuildAliases
    {
        /// <summary>
        /// Reads build configuration from script arguments
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        [CakeMethodAlias]
        public static void BuildConfigure(this ICakeContext context)
        {
            // Build args
            Build.Target = 
                CoerceTarget(context.Argument("target", Build.Target));
            
            Build.Configuration = 
                context.Argument("configuration", Build.Configuration);
            
            Build.Verbosity = 
                context.Argument("verbosity", Build.Verbosity);
            
            Build.Local = 
                context.Argument("local", Build.Local);

            // Check args
            Build.CheckStagedChanges = 
                context.Argument("check-staged", Build.CheckStagedChanges);
            
            Build.CheckUncommittedChanges = 
                context.Argument("check-uncommitted", Build.CheckUncommittedChanges);
            
            Build.CheckUntrackedFiles = 
                context.Argument("check-untracked", Build.CheckUntrackedFiles);

            // Git args
            Build.GitRoot = 
                context.GitFindRootFromPath(context.MakeAbsolute(context.Directory(".")));
            
            Build.GitUserName = 
                context.Argument("git-username", Build.GitUserName);
            
            Build.GitEmail = 
                context.Argument("git-email", Build.GitEmail);
            
            Build.GitRemoteName = 
                context.Argument("git-remote", Build.GitRemoteName);
            
            Build.EnableCommits = 
                context.Argument("enable-commits", Build.EnableCommits);
            
            Build.EnableTags = 
                context.Argument("enable-tags", Build.EnableTags);
            
            Build.EnablePush = 
                context.Argument("enable-push", Build.EnablePush);

            // NuGet args
            Build.NuGetSource = 
                context.Argument(
                    "nuget-source", 
                    context.EnvironmentVariable("NUGET_SOURCE") ?? Build.NuGetSource);
            
            Build.NuGetLocalSource = 
                context.Argument(
                    "nuget-local-source", 
                    context.EnvironmentVariable("NUGET_LOCAL_SOURCE") ?? Build.NuGetLocalSource);
            
            Build.NuGetApiKey = 
                context.Argument("nuget-api-key", context.EnvironmentVariable("NUGET_API_KEY"));

            // Artefact args
            Build.ArtefactsRepository = 
                context.Argument(
                    "artefacts-repo", 
                    context.EnvironmentVariable("ARTEFACTS_REPO") ?? Build.ArtefactsRepository);

            Build.ArtefactsLocalRepository = 
                context.Argument(
                    "artefacts-local-repo",
                    context.EnvironmentVariable("ARTEFACTS_LOCAL_REPO") ?? Build.ArtefactsLocalRepository);

            // Squirrel args
            Build.SquirrelCentralRepository = 
                context.Argument(
                    "squirrel-repo",
                    context.EnvironmentVariable("SQUIRREL_REPO") ?? Build.SquirrelCentralRepository);

            Build.SquirrelLocalRepository = 
                context.Argument(
                    "squirrel-local-repo",
                    context.EnvironmentVariable("SQUIRREL_LOCAL_REPO") ?? Build.SquirrelLocalRepository);
        }


        /// <summary>
        /// Helper method to coerce the run-target to a valid target name
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string CoerceTarget(string target)
        {
            switch (target)
            {
                case "Info":
                case "Check":
                case "InfoOnly":
                case "BuildAll":
                case "TestAll":
                case "PackageAll":
                case "RunAll":
                    return $"Build-{target}";

                default:
                    return target;
            }
        }


        /// <summary>
        /// Prints out build configuration information
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <example>
        /// <code>
        /// Task("Info").Does(() => BuildInfo());
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void BuildInfo(this ICakeContext context)
        {
            context.Information("\nScript Options:");
            context.Information($"--target                  {Build.Target}");
            context.Information($"--configuration           {Build.Configuration}");
            context.Information($"--verbosity               {Build.Verbosity}");
            context.Information($"--local                   {Build.Local}");

            context.Information("\nCheck Options:");
            context.Information($"--check-staged            {Build.CheckStagedChanges}");
            context.Information($"--check-uncommitted       {Build.CheckUncommittedChanges}");
            context.Information($"--check-untracked         {Build.CheckUntrackedFiles}");

            context.Information("\nGit Options:");
            context.Information($"--git-root                {Build.GitRoot}");
            context.Information($"--git-username            {Build.GitUserName}");
            context.Information($"--git-email               {Build.GitEmail}");
            context.Information($"--git-remote              {Build.GitRemoteName}");
            context.Information($"--enable-commits          {Build.EnableCommits}");
            context.Information($"--enable-tags             {Build.EnableTags}");
            context.Information($"--enable-push             {Build.EnablePush}");

            context.Information("\nNuget Options:");
            context.Information($"--nuget-source            {Build.NuGetSource}");
            context.Information($"--nuget-local-source      {Build.NuGetLocalSource}");
            context.Information($"--nuget-api-key           {Build.NuGetApiKey}");

            context.Information("\nArtefacts Options:");
            context.Information($"--artefacts-repo          {Build.ArtefactsRepository}");
            context.Information($"--artefacts-local-repo    {Build.ArtefactsLocalRepository}");

            context.Information("\nSquirrel Options:");
            context.Information($"--squirrel-repo           {Build.SquirrelCentralRepository}");
            context.Information($"--squirrel-local-repo     {Build.SquirrelLocalRepository}");

            context.Information("\nConfigured Recipes:");
            foreach (var recipe in Build.RecipeBuilds)
                context.Information($"  {recipe.Id} - {recipe.Name}");
        }


        /// <summary>
        /// Prints out recipe build status information
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <example>
        /// <code>
        /// Task("Status").Does(() => BuildStatus());
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void BuildStatus(this ICakeContext context)
        {
            var successful = Build.RecipeBuilds.Where(b => !b.Errored && !b.SkipRemainingTasks);
            if (successful.Any())
            {
                context.Information("Successful Builds:");
                foreach (var recipe in successful)
                    context.Information($"{recipe} completed successfully");
                context.Information("");
            }

            var skipped = Build.RecipeBuilds.Where(b => !b.Errored && b.SkipRemainingTasks);
            if (skipped.Any())
            {
                context.Information("Skipped Builds:");
                foreach (var recipe in skipped)
                    context.Information($"{recipe} skipped");
                context.Information("");
            }

            var errored = Build.RecipeBuilds.Where(b => b.Errored);
            if (errored.Any())
            {
                context.Information("Errored Builds:");
                foreach (var recipe in errored)
                    context.Information($"{recipe} - {recipe.ErroredTaskName}\n{recipe.Exception.Message}");
                context.Information("");
            }
        }


        /// <summary>
        /// Pushes changes to the git remote
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        [CakeMethodAlias]
        public static void BuildPush(this ICakeContext context)
            // ToDo: check and skip if single recipe failure
            => context.PushChanges(new PushSettings {
                Remote = Build.GitRemoteName,
                Tags = Build.EnableTags });
    }
}
