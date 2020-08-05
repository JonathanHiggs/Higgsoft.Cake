namespace Higgsoft.Cake.Commit
{
    /// <summary>
    /// Settings to control pushing commits to an upstream remote
    /// </summary>
    public class PushSettings
    {
        /// <summary>
        /// Gets and sets the name of the remote
        /// </summary>
        public string Remote { get; set; } = "origin";


        /// <summary>
        /// Get and sets a flag that determines whether tags are pushed
        /// </summary>
        public bool Tags { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that forces the push
        /// </summary>
        public bool Force { get; set; } = false;
    }
}
