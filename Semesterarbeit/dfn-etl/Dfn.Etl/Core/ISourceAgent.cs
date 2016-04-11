using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents the ActionBlock equivalent for sources.
    /// It is an independent producer of values of TOut.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface ISourceAgent<out TOut> : ISourceAgent
    {
        /// <summary>
        /// Starts producing values asynchronously.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Links itself to the given target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="linkOptions"></param>
        /// <param name="predicate"></param>
        void LinkTo(ITargetBlock<TOut> target, DataflowLinkOptions linkOptions, Predicate<TOut> predicate);
    }

    public interface ISourceAgent {}
}