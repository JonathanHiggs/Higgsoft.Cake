using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Higgsoft.Cake.Check
{
    /// <summary>
    /// Represents the result of performing a pre-build check
    /// </summary>
    public interface ICheckResult
    {
        /// <summary>
        /// Gets the check source
        /// </summary>
        string Source { get; }


        /// <summary>
        /// Gets a value that determines whether the check passed or failed
        /// </summary>
        bool Failed { get; }
    }
}
