using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging
{
    public class SkipBrokenMessagesDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> : DataflowNetworkDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>
        where TOutputMsg : IDataflowMessage<TOutput>
        where TInputMsg : IDataflowMessage<TInput>
    {
        private volatile int m_NumMessagesProcessed;
        private volatile int m_NumBrokenMessages;

        public SkipBrokenMessagesDecoratorTask(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> innerTask, IIdasDataflowNetwork network, ILogAgent logAgent)
            : base(innerTask, network, logAgent)
        {
        }

        protected override IEnumerable<TOutputMsg> DecorateOutputMessages(IEnumerable<TOutputMsg> outputMsgs)
        {
            if (outputMsgs == null)
                yield break;

            var e = outputMsgs.GetEnumerator();
            while (true)
            {
                if (!e.MoveNext())
                    yield break;

                //Get the next output message
                TOutputMsg outputMsg = e.Current;

                Interlocked.Increment(ref m_NumMessagesProcessed);

                if (outputMsg.IsBroken)
                {
                    Interlocked.Increment(ref m_NumBrokenMessages);

                    //Don't use the broken messages... skip those!
                    continue;
                }

                yield return outputMsg;
            }
        }
    }
}