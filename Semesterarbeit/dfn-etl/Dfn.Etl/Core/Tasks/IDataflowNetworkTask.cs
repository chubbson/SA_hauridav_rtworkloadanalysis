using System;
using System.Collections.Generic;

namespace Dfn.Etl.Core.Tasks
{
    public interface IDataflowNetworkTask : IDisposable
    {
        string Name { get; }
        DataflowNetworkConstituent TaskType { get; }

        void BeginProcessingTask();
        void EndProcessingTask();
    }

    public interface IDataflowNetworkTask<in TInput, in TInputMsg, out TOutput, out TOutputMsg> : IDataflowNetworkTask
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        TOutputMsg CreateOutputMessage();

        IEnumerable<TOutputMsg> Process(IEnumerable<TInputMsg> inputMsgs);
    }

    public interface IDataflowNetworkTask<in TInput, out TOutput> : IDataflowNetworkTask<TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>
    {
    }
}