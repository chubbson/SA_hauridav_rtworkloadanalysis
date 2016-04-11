using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Core.Tasks;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Provides a group of methods that wrap the given dataflow network constituent into an exception handler.
    /// </summary>
    public static class DataflowExceptionDecoration
    {
        public static IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> WithExceptionHandler<TInput, TInputMsg, TOutput, TOutputMsg>(this IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> decoratedSource, IIdasDataflowNetwork network, ILogAgent logAgent)
            where TInputMsg : IDataflowMessage<TInput>
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            return new ExceptionDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>(decoratedSource, network, logAgent);
        }

        public static ISourceFunctor<TOut> WithExceptionHandler<TOut>(this ISource<TOut> decoratedSource, ILogAgent logAgent, ICancelNetwork cancel)
        {
            return new ExceptionDecoratorSource<TOut>(decoratedSource, logAgent, cancel);
        }

        public static ITransformationFunctor<TIn, TOut> WithExceptionHandler<TIn, TOut>(this ITransformation<TIn, TOut> decoratedTransformation, ILogAgent log, ICancelNetwork cancel)
        {
            return new ExceptionDecoratorTransformation<TIn, TOut>(decoratedTransformation, log, cancel);
        }

        public static ITransformationBatchedFunctor<TIn, TOut> WithExceptionHandler<TIn, TOut>(this ITransformationBatched<TIn, TOut> decoratedTransformation, ILogAgent log, ICancelNetwork cancel)
        {
            return new ExceptionDecoratorTransformationBatched<TIn, TOut>(decoratedTransformation, log, cancel);
        }

        public static ITransformManyFunctor<TIn, TOut> WithExceptionHandler<TIn, TOut>(this ITransformMany<TIn, TOut> decoratedTransformation, ILogAgent log, ICancelNetwork cancel)
        {
            return new ExceptionDecoratorTransformMany<TIn, TOut>(decoratedTransformation, log, cancel);
        }

        public static ITargetFunctor<TIn> WithExceptionHandler<TIn>(this ITarget<TIn> decoratedTarget, ILogAgent log, ICancelNetwork cancel)
        {
            return new ExceptionDecoratorTarget<TIn>(decoratedTarget, log, cancel);
        }

        public static ITargetBatchedFunctor<TIn> WithExceptionHandler<TIn>(this ITargetBatched<TIn> decoratedTarget, ILogAgent log, ICancelNetwork cancel)
        {
            return new ExceptionDecoratorTargetBatched<TIn>(decoratedTarget, log, cancel);
        }
    }
}
