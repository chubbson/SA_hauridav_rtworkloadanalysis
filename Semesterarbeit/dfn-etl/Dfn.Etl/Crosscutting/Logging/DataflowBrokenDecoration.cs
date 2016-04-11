using Dfn.Etl.Core;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging
{
    public static class DataflowBrokenDecoration
    {
        public static IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> SkipBroken<TInput, TInputMsg, TOutput, TOutputMsg>(this IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> decoratedSource, IIdasDataflowNetwork network, ILogAgent logAgent)
            where TInputMsg : IDataflowMessage<TInput>
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            return new SkipBrokenMessagesDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>(decoratedSource, network, logAgent);
        }
    }
}