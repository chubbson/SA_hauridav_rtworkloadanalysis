using System.Collections.Generic;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a producer of a dataflow network.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface ISource<out TOut>
    {
        /// <summary>
        /// Describes the source.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Produces items.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TOut> Pull();
    }
}