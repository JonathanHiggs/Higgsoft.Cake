using System.Linq;

using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Annotations;

using Higgsoft.Cake.Commit;

namespace Higgsoft.Cake.Recipes
{
    [CakeAliasCategory("Higgsoft.Build")]
    public static class BuildAliases
    {
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
            => context.PushChanges(new PushSettings {
                Remote = Build.GitRemoteName,
                Tags = Build.EnableTags });
    }
}
