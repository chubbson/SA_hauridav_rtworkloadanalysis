using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    /// <summary>
    /// Decorates the given target with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class StatisticsLogDecoratorTarget<TIn> : StatisticsLoggerBase, ITargetFunctor<TIn>
    {
        private readonly ITargetFunctor<TIn> m_DecoratedTarget;

        public StatisticsLogDecoratorTarget(ITargetFunctor<TIn> decoratedTarget, ILogAgent logAgent)
            : base(logAgent, decoratedTarget.Title, DataflowNetworkConstituent.Target)
        {
            m_DecoratedTarget = decoratedTarget;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTarget.Title;
            }
        }

        public void Push(IDataflowMessage<TIn> item)
        {
            Interlocked.Increment(ref NumProcessedMessages);
            m_DecoratedTarget.Push(item);
            if (item.IsBroken)
            {
                Interlocked.Increment(ref NumBrokenMessages);
            }
        }
    }
}