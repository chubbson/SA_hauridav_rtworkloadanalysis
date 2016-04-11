using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    /// <summary>
    /// Decorates the given transformation with statistics logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class StatisticsLogDecoratorTransformationBatched<TIn, TOut> : StatisticsLoggerBase, ITransformationBatchedFunctor<TIn, TOut>
    {
        private readonly ITransformationBatchedFunctor<TIn, TOut> m_DecoratedTransformationBatched;

        public StatisticsLogDecoratorTransformationBatched(ITransformationBatchedFunctor<TIn, TOut> decoratedTransformationBatched, ILogAgent logAgent)
            : base(logAgent, decoratedTransformationBatched.Title, DataflowNetworkConstituent.TransformationBatched)
        {
            m_DecoratedTransformationBatched = decoratedTransformationBatched;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTransformationBatched.Title;
            }
        }

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn>[] items)
        {
            Interlocked.Increment(ref NumProcessedMessages);
            IDataflowMessage<TOut> result = m_DecoratedTransformationBatched.Transform(items);

            if (result.IsBroken)
            {
                Interlocked.Increment(ref NumBrokenMessages);
            }
            return result;
        }
    }
}
