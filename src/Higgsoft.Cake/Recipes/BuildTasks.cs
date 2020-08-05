using Cake.Core;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Build tasks
    /// </summary>
    public class BuildTasks
    {
        /// <summary>
        /// Gets and sets the info task builder
        /// </summary>
        public CakeTaskBuilder Info { get; set; }


        /// <summary>
        /// Gets and sets the check task builder
        /// </summary>
        public CakeTaskBuilder Check { get; set; }


        /// <summary>
        /// Gets and sets the push task builder
        /// </summary>
        public CakeTaskBuilder Push { get; set; }
    }
}
