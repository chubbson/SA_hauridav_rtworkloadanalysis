using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging
{
    public static class DataflowEmptyDecoration
    {
        public static ISourceFunctor<TOut> SkipEmpty<TOut>(this ISourceFunctor<TOut> decoratedSource)
        {
            return new SkipEmptyDecoratorSource<TOut>(decoratedSource);
        }

        public static ITransformationFunctor<TIn, TOut> SkipEmpty<TIn, TOut>(this ITransformationFunctor<TIn, TOut> decoratedTransformation)
        {
            return new SkipEmptyDecoratorTransformation<TIn, TOut>(decoratedTransformation);
        }

        public static IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> SkipEmpty<TInput, TInputMsg, TOutput, TOutputMsg>(this IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> decoratedSource, IIdasDataflowNetwork network, ILogAgent logAgent)
            where TInputMsg : IDataflowMessage<TInput>
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            return new SkipEmptyMessagesDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>(decoratedSource, network, logAgent);
        }
    }
}