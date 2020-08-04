using System;

using Cake.Common.Diagnostics;
using Cake.Common.Solution.Project.Properties;
using Cake.Common.Tools.NUnit;
using Cake.Core;
using Cake.Core.Annotations;

using Higgsoft.Cake.Check;
using Higgsoft.Cake.Commit;
using Higgsoft.Cake.ReleaseNotes;
using Higgsoft.Cake.Versions;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Extension methods common across all <see cref="Recipe"/>
    /// </summary>
    [CakeAliasCategory("Higgsoft.Cake.Recipes")]
    public static class RecipeAliases
    {
        /// <summary>
        /// Performs pre-build checks to ensure a consistent build state
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeCheck(this ICakeContext context, Recipe recipe)
        {
            context.Check(recipe.CheckSettings);

            recipe.SkipRemainingTasks =
                recipe.PrepareReleaseNotes && !context.ReleaseNotesUpdated(recipe.ReleaseNotesSettings);
        }


        /// <summary>
        /// Parses the build version for the recipe
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeVersion(this ICakeContext context, Recipe recipe)
        {
            recipe.Version = context.ParseVersionFrom(recipe.ReleaseNotesVNextFile);
            context.Information($"Building version: {recipe.Version}");
        }


        /// <summary>
        /// Prepends the release notes from the v-next file
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeReleaseNotes(this ICakeContext context, Recipe recipe)
            => recipe.ReleaseNotes.AddRange(context.UpdateReleaseNotes(recipe.ReleaseNotesSettings));


        /// <summary>
        /// Updates the assembly info file with the recipe version
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeAssemblyInfo(this ICakeContext context, Recipe recipe)
            => context.CreateAssemblyInfo(recipe.AssemblyInfoFile, recipe.AssemblyInfoSettings);


        /// <summary>
        /// Executes recipe unit tests
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeTest(this ICakeContext context, Recipe recipe)
            => context.NUnit3(
                $"{recipe.SolutionDirectory}/**/bin/{Build.Configuration}/*.UnitTests.dll",
                recipe.NUnitSettings);


        /// <summary>
        /// Commits changes to files made during the build
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeCommit(this ICakeContext context, Recipe recipe)
            => context.CommitChanges(recipe.CommitSettings);


        /// <summary>
        /// Performs clean-up logic
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        [CakeMethodAlias]
        public static void RecipeCleanUp(this ICakeContext context, Recipe recipe)
            => context.RevertChanges(recipe.RevertSettings);


        /// <summary>
        /// Sets the recipe to the error state and aborts the remainder of the tasks
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="recipe">Recipe configuration</param>
        /// <param name="task">Task that threw and error</param>
        /// <param name="ex">Exception thrown during task</param>
        [CakeMethodAlias]
        public static void RecipeOnError(
            this ICakeContext context,
            Recipe recipe,
            CakeTaskBuilder task,
            Exception ex)
        {
            recipe.SetError(task, ex);
            context.Error(ex);
        }
    }
}
