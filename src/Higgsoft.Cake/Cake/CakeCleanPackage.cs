using Cake.Core.IO;

namespace Higgsoft.Cake.Cake
{
    public class CakeCleanPackageSettings
    {
        /// <summary>
        /// Gets and sets the NuGet package identity
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// Gets and sets the version of the NuGet package
        /// </summary>
        public string Version { get; set; }


        /// <summary>
        /// Gets and sets the path to the tools directory
        /// </summary>
        public DirectoryPath ToolsDirectory { get; set; }
    }
}
