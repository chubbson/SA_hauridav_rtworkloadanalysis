using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    /// <summary>
    /// Provides a group of methods that decorate dataflow network with logging capabilities.
    /// </summary>
    public static class DataflowStatisticsLoggingDecoration
    {
        public static StatisticsLogDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> 
            WithStatistics<TInput, TInputMsg, TOutput, TOutputMsg>
                ( this IDataflowNetworkTask<TInput, TInputMsg, TOutput
                , TOutputMsg> decoratedSource
                , IIdasDataflowNetwork network
                , ILogAgent logAgent )
            where TInputMsg : IDataflowMessage<TInput>
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            return new StatisticsLogDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>(decoratedSource, network, logAgent);
        }

        public static ITransformationFunctor<TIn, TOut> WithStatistics<TIn, TOut>(this ITransformationFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            return new StatisticsLogDecoratorTransformation<TIn, TOut>(decoratedTransformation, logAgent);
        }

        public static ITransformationBatchedFunctor<TIn, TOut> WithStatistics<TIn, TOut>(this ITransformationBatchedFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            return new StatisticsLogDecoratorTransformationBatched<TIn, TOut>(decoratedTransformation, logAgent);
        }

        public static ITransformManyFunctor<TIn, TOut> WithStatistics<TIn, TOut>(this ITransformManyFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            return new StatisticsLogDecoratorTransformMany<TIn, TOut>(decoratedTransformation, logAgent);
        }

        public static ITargetFunctor<TIn> WithStatistics<TIn>(this ITargetFunctor<TIn> decoratedTarget, ILogAgent logAgent)
        {
            return new StatisticsLogDecoratorTarget<TIn>(decoratedTarget, logAgent);
        }

        public static ISourceFunctor<TIn> WithStatistics<TIn>(this ISourceFunctor<TIn> decoratedSource, ILogAgent logAgent)
        {
            return new StatisticsLogDecoratorSource<TIn>(decoratedSource, logAgent);
        }

        public static ITargetBatchedFunctor<TIn> WithStatistics<TIn>(this ITargetBatchedFunctor<TIn> decoratedTarget, ILogAgent logAgent)
        {
            return new StatisticsLogDecoratorTargetBatched<TIn>(decoratedTarget, logAgent);
        }
    }
}
