
using Cake.Core;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Build targets
    /// </summary>
    public class BuildTargets
    {
        /// <summary>
        /// Gets and sets the info-only task builder
        /// </summary>
        public CakeTaskBuilder InfoOnly { get; set; }


        /// <summary>
        /// Gets and sets the build-all task builder
        /// </summary>
        public CakeTaskBuilder BuildAll { get; set; }


        /// <summary>
        /// Gets and sets the test-all task builder
        /// </summary>
        public CakeTaskBuilder TestAll { get; set; }


        /// <summary>
        /// Gets and sets the package-all task builder
        /// </summary>
        public CakeTaskBuilder PackageAll { get; set; }


        /// <summary>
        /// Gets and sets the run-all task builder
        /// </summary>
        public CakeTaskBuilder RunAll { get; set; }
    }
}
