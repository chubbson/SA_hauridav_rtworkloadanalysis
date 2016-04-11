using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Dfn.Etl.Core
{
    public static class CustomBlocks
    {
        /// <summary>
        /// Creates the throttled transform many.
        /// </summary>
        /// <typeparam name="TIn">The type of the in.</typeparam>
        /// <typeparam name="TOut">The type of the out.</typeparam>
        /// <param name="transform">The transform.</param>
        /// <param name="dataflowBlockOptions">The dataflow block options.</param>
        /// <returns></returns>
        public static IPropagatorBlock<TIn, TOut> CreateThrottledTransformMany<TIn, TOut>(
            Func<TIn, IEnumerable<TOut>> transform,
            ExecutionDataflowBlockOptions dataflowBlockOptions
            )
        {
            var source = new BufferBlock<TOut>(dataflowBlockOptions);
            var target = new ActionBlock<TIn>(@in => {
                foreach (var @out in transform(@in))
                {
                    source.SendAsync(@out).Wait();
                }
            });
            target.Completion.ContinueWith(completion => {
                if (completion.IsFaulted)
                {
                    ((IDataflowBlock)source).Fault(completion.Exception);
                }
                else
                {
                    source.Complete();
                }
            });
            return DataflowBlock.Encapsulate(target, source);
        }
    }
}