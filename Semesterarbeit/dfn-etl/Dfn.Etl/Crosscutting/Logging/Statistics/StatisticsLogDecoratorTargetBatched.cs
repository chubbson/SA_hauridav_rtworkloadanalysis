using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    /// <summary>
    /// Decorates the given batched target with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class StatisticsLogDecoratorTargetBatched<TIn> : StatisticsLoggerBase, ITargetBatchedFunctor<TIn>
    {
        private readonly ITargetBatchedFunctor<TIn> m_DecoratedTarget;

        public StatisticsLogDecoratorTargetBatched(ITargetBatchedFunctor<TIn> decoratedTarget, ILogAgent logAgent)
            : base(logAgent, decoratedTarget.Title, DataflowNetworkConstituent.TargetBatched)
        {
            m_DecoratedTarget = decoratedTarget;
            // Can't tell, how many messages are broken
            Interlocked.Decrement(ref NumBrokenMessages);
        }

        public string Title
        {
            get
            {
                return m_DecoratedTarget.Title;
            }
        }

        public void Push(IDataflowMessage<TIn>[] items)
        {
            Interlocked.Add(ref NumProcessedMessages, items.LongLength);
            m_DecoratedTarget.Push(items);
        }
    }
}
