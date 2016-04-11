using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    /// <summary>
    /// Decorates the given target with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class StatisticsLogDecoratorSource<TOut> : StatisticsLoggerBase, ISourceFunctor<TOut>
    {
        private readonly ISourceFunctor<TOut> m_DecoratedFunctor;

        public StatisticsLogDecoratorSource(ISourceFunctor<TOut> decoreatedFunctor, ILogAgent logAgent)
            : base(logAgent, decoreatedFunctor.Title, DataflowNetworkConstituent.Source)
        {
            m_DecoratedFunctor = decoreatedFunctor;
        }

        public string Title
        {
            get
            {
                return m_DecoratedFunctor.Title;
            }
        }

        public IEnumerable<IDataflowMessage<TOut>> Pull()
        {
            foreach (var dfMsg in m_DecoratedFunctor.Pull())
            {
                Interlocked.Increment(ref NumProcessedMessages);
                if (dfMsg.IsBroken)
                {
                    Interlocked.Increment(ref NumBrokenMessages);
                }
                yield return dfMsg;
            }
        }
    }
}