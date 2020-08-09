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


        /// <summary>
        /// Gets and sets a struct that holds the names of all the tasks
        /// </summary>
        public TaskNames Names { get; set; }


        /// <summary>
        /// Struct that holds the names of all the tasks
        /// </summary>
        public readonly struct TaskNames
        {
            private readonly string id;


            /// <summary>
            /// Initializes a new instance of <see cref="TaskNames"/>
            /// </summary>
            /// <param name="id"></param>
            public TaskNames(string id)
            {
                this.id = id;
            }


            /// <summary>
            /// Gets the name of the info task
            /// </summary>
            public string Info => $"{id}-Info";


            /// <summary>
            /// Gets the name of the setup task
            /// </summary>
            public string Setup => $"{id}-Setup";


            /// <summary>
            /// Gets the name of the check task
            /// </summary>
            public string Check => $"{id}-Check";


            /// <summary>
            /// Gets the name of the version task
            /// </summary>
            public string Version => $"{id}-Version";


            /// <summary>
            /// Gets the name of the release notes task
            /// </summary>
            public string ReleaseNotes => $"{id}-ReleaseNotes";


            /// <summary>
            /// Gets the name of the assembly info task
            /// </summary>
            public string AssemblyInfo => $"{id}-AssemblyInfo";


            /// <summary>
            /// Gets the name of the clean task
            /// </summary>
            public string Clean => $"{id}-Clean";


            /// <summary>
            /// Gets the name of the pre-build task
            /// </summary>
            public string PreBuild => $"{id}-PreBuild";


            /// <summary>
            /// Gets the name of the build task
            /// </summary>
            public string Build => $"{id}-Build";


            /// <summary>
            /// Gets the name of the post-build task
            /// </summary>
            public string PostBuild => $"{id}-PostBuild";


            /// <summary>
            /// Gets the name of the test task
            /// </summary>
            public string Test => $"{id}-Test";


            /// <summary>
            /// Gets the name of the publish task
            /// </summary>
            public string Publish => $"{id}-Publish";


            /// <summary>
            /// Gets the name of the package task
            /// </summary>
            public string Package => $"{id}-Package";


            /// <summary>
            /// Gets the name of the commit task
            /// </summary>
            public string Commit => $"{id}-Commit";


            /// <summary>
            /// Gets the name of the push task
            /// </summary>
            public string Push => $"{id}-Push";


            /// <summary>
            /// Gets the name of the cleup-up task
            /// </summary>
            public string CleanUp => $"{id}-CleanUp";
        }
    }
}
