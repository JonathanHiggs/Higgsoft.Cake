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
    [CakeAliasCategory("Higgsoft.Cake.Recipes")]
    public static class RecipeAliases
    {
        [CakeMethodAlias]
        public static void RecipeCheck(this ICakeContext context, Recipe recipe)
        {
            context.Check(recipe.CheckSettings);

            recipe.SkipRemainingTasks =
                recipe.PrepareReleaseNotes && !context.ReleaseNotesUpdated(recipe.ReleaseNotesSettings);
        }


        [CakeMethodAlias]
        public static void RecipeVersion(this ICakeContext context, Recipe recipe)
            => recipe.Version = context.ParseVersionFrom(recipe.ReleaseNotesVNextFile);


        [CakeMethodAlias]
        public static void RecipeReleaseNotes(this ICakeContext context, Recipe recipe)
            => recipe.ReleaseNotes.AddRange(context.UpdateReleaseNotes(recipe.ReleaseNotesSettings));


        [CakeMethodAlias]
        public static void RecipeAssemblyInfo(this ICakeContext context, Recipe recipe)
            => context.CreateAssemblyInfo(recipe.AssemblyInfoFile, recipe.AssemblyInfoSettings);


        [CakeMethodAlias]
        public static void RecipeTest(this ICakeContext context, Recipe recipe)
            => context.NUnit3(
                $"{recipe.SolutionDirectory}/**/bin/{Build.Configuration}/*.UnitTests.dll",
                recipe.NUnitSettings);


        [CakeMethodAlias]
        public static void RecipeCommit(this ICakeContext context, Recipe recipe)
            => context.CommitChanges(recipe.CommitSettings);


        [CakeMethodAlias]
        public static void RecipeCleanUp(this ICakeContext context, Recipe recipe)
            => context.RevertChanges(recipe.RevertSettings);


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
