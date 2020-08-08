using Cake.Core.IO;

namespace Higgsoft.Cake.Utils
{
    /// <summary>
    /// Settings to control manually installing a nuget package into cake tools directory
    /// </summary>
    public class CakeInstallPackageSettings

    {
        /// <summary>
        /// Gets and sets the identity of the NuGet package
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// Gets and sets the version of the NuGet package
        /// </summary>
        public string Version { get; set; }


        /// <summary>
        /// Gets and sets the path to the nuget package
        /// </summary>
        public DirectoryPath NuGetDirectory { get; set; }


        /// <summary>
        /// Gets and sets the path to the tools directory
        /// </summary>
        public DirectoryPath ToolsDirectory { get; set; }


        /// <summary>
        /// Gets and sets a flag that determines whether the package is installed as an addin
        /// </summary>
        public bool AsAddin { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that determines whether the package is installed as a tool
        /// </summary>
        public bool AsTool { get; set; }
    }
}
