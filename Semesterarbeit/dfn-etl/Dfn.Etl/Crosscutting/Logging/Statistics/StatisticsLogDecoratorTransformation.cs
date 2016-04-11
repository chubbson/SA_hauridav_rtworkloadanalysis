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
    public sealed class StatisticsLogDecoratorTransformation<TIn, TOut> : StatisticsLoggerBase, ITransformationFunctor<TIn, TOut>
    {
        private readonly ITransformationFunctor<TIn, TOut> m_DecoratedTransformation;

        public StatisticsLogDecoratorTransformation(ITransformationFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
            : base(logAgent, decoratedTransformation.Title, DataflowNetworkConstituent.Transformation)
        {
            m_DecoratedTransformation = decoratedTransformation;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTransformation.Title;
            }
        }

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn> item)
        {
            Interlocked.Increment(ref NumProcessedMessages);
            IDataflowMessage<TOut> result = m_DecoratedTransformation.Transform(item);

            if (result.IsBroken)
            {
                Interlocked.Increment(ref NumBrokenMessages);
            }
            return result;
        }
    }
}
