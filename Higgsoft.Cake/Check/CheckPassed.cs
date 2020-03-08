using System.Runtime.CompilerServices;

namespace Higgsoft.Cake.Check
{
    /// <summary>
    /// Represents a passing check
    /// </summary>
    public class CheckPassed : ICheckResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CheckPassed"/>
        /// </summary>
        /// <param name="source">Name of the check</param>
        public CheckPassed([CallerMemberName] string source = "")
            => Source = source ?? string.Empty;


        /// <summary>
        /// Gets the check source
        /// </summary>
        public string Source { get; }


        /// <summary>
        /// Gets a value that determines whether the check failed
        /// </summary>
        public bool Failed => false;
    }
}
