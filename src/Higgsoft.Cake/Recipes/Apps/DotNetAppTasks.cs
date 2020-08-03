using Cake.Core;

namespace Higgsoft.Cake.Recipes.Apps
{
    /// <summary>
    /// Build tasks for a dotnet library nuget package
    /// </summary>
    public class DotNetAppTasks : RecipeTasks
    {
        /// <summary>
        /// Gets and sets the build <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Build { get; set; }


        /// <summary>
        /// Gets and sets the publish <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Publish { get; set; }
    }
}
