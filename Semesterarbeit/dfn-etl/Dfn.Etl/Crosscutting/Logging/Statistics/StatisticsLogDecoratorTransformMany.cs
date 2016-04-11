using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    /// <summary>
    /// Decorates the given one-to-many transformation with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class StatisticsLogDecoratorTransformMany<TIn, TOut> : StatisticsLoggerBase, ITransformManyFunctor<TIn, TOut>
    {
        private readonly ITransformManyFunctor<TIn, TOut> m_Decorated;

        public StatisticsLogDecoratorTransformMany(ITransformManyFunctor<TIn, TOut> decorated, ILogAgent logAgent)
            : base(logAgent, decorated.Title, DataflowNetworkConstituent.TransformMany)
        {
            m_Decorated = decorated;
            // Note: We cannot know the number of broken messages, so we set it to -1;
            Interlocked.Decrement(ref NumBrokenMessages);
        }

        public string Title
        {
            get { return m_Decorated.Title; }
        }

        public IEnumerable<IDataflowMessage<TOut>> TransformMany(IDataflowMessage<TIn> item)
        {
            var result = m_Decorated.TransformMany(item);
            Interlocked.Increment(ref NumProcessedMessages);
            return result;
        }
    }
}