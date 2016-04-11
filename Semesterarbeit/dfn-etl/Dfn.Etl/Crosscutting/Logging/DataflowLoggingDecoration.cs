using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Provides a group of methods that decorate dataflow network with logging capabilities.
    /// </summary>
    public static class DataflowLoggingDecoration
    {
        public static IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> WithLogging<TInput, TInputMsg, TOutput, TOutputMsg>(this IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> decoratedSource, IIdasDataflowNetwork network, ILogAgent logAgent)
            where TInputMsg : IDataflowMessage<TInput>
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            return new LoggingDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>(decoratedSource, network, logAgent);
        }

        public static ISourceFunctor<TOut> WithLogging<TOut>(this ISourceFunctor<TOut> decoratedSource, ILogAgent logAgent)
        {
            return new LogDecoratorSource<TOut>(decoratedSource, logAgent);
        }

        public static ITransformationFunctor<TIn, TOut> WithLogging<TIn, TOut>(this ITransformationFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            return new LogDecoratorTransformation<TIn, TOut>(decoratedTransformation, logAgent);
        }

        public static ITransformationBatchedFunctor<TIn, TOut> WithLogging<TIn, TOut>(this ITransformationBatchedFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            return new LogDecoratorTransformationBatched<TIn, TOut>(decoratedTransformation, logAgent);
        }

        public static ITransformManyFunctor<TIn, TOut> WithLogging<TIn, TOut>(this ITransformManyFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            return new LogDecoratorTransformMany<TIn, TOut>(decoratedTransformation, logAgent);
        }

        public static ITargetFunctor<TIn> WithLogging<TIn>(this ITargetFunctor<TIn> decoratedTarget, ILogAgent logAgent)
        {
            return new LogDecoratorTarget<TIn>(decoratedTarget, logAgent);
        }

        public static ITargetBatchedFunctor<TIn> WithLogging<TIn>(this ITargetBatchedFunctor<TIn> decoratedTarget, ILogAgent logAgent)
        {
            return new LogDecoratorTargetBatched<TIn>(decoratedTarget, logAgent);
        }
    }
}
