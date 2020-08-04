using Cake.Core;

namespace Higgsoft.Cake.Recipes.Libs
{
    /// <summary>
    /// Build tasks for a dotnet library nuget package
    /// </summary>
    public class DotNetLibTasks : RecipeTasks
    {
        /// <summary>
        /// Gets and sets the restore <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Restore { get; set; }


        /// <summary>
        /// Gets and sets the build <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Build { get; set; }


        /// <summary>
        /// Gets and sets the publish <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Publish { get; set; }


        // ToDo: Add docs task
    }
}
