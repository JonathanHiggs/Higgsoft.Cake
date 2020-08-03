using Cake.Core;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Shared build tasks for all recipes
    /// </summary>
    public class RecipeTasks
    {
        /// <summary>
        /// Gets and sets the info <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Info { get; set; }


        /// <summary>
        /// Gets and sets the setup <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Setup { get; set; }


        /// <summary>
        /// Gets and sets the check <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Check { get; set; }


        /// <summary>
        /// Gets and sets the version <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Version { get; set; }


        /// <summary>
        /// Gets and sets the release notes <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder ReleaseNotes { get; set; }


        /// <summary>
        /// Gets and sets the assembly info <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder AssemblyInfo { get; set; }


        /// <summary>
        /// Gets and sets the clean <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Clean { get; set; }


        /// <summary>
        /// Gets and sets the pre-build <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder PreBuild { get; set; }


        /// <summary>
        /// Gets and sets the post-build <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder PostBuild { get; set; }


        /// <summary>
        /// Gets and sets the test <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Test { get; set; }


        /// <summary>
        /// Gets and sets the package <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Package { get; set; }


        /// <summary>
        /// Gets and sets the commit <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Commit { get; set; }


        /// <summary>
        /// Gets and sets the push <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder Push { get; set; }


        /// <summary>
        /// Gets and sets the clean-up <see cref="CakeTaskBuilder"/>
        /// </summary>
        public CakeTaskBuilder CleanUp { get; set; }
    }
}
