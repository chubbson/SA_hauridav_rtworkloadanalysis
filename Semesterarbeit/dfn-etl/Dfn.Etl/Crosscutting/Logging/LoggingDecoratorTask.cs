using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging
{
    public class LoggingDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> : DataflowNetworkDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>
        where TOutputMsg : IDataflowMessage<TOutput>
        where TInputMsg : IDataflowMessage<TInput>
    {
        private volatile int m_NumMessagesProcessed;
        private volatile int m_NumBrokenMsgs;

        public int NumMessagesSeen
        {
            get { return m_NumMessagesProcessed; }
        }

        public int NumBrokenMsgs
        {
            get { return m_NumBrokenMsgs; }
        }

        public int NumGoodMsgs
        {
            get { return m_NumMessagesProcessed - m_NumBrokenMsgs; }
        }

        public LoggingDecoratorTask(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> innerTask, IIdasDataflowNetwork network, ILogAgent logAgent)
            : base(innerTask, network, logAgent)
        {
        }

        protected override IEnumerable<TInputMsg> DecorateInputMessages(IEnumerable<TInputMsg> inputMsgs)
        {
            var e = inputMsgs.GetEnumerator();
            while (true)
            {
                if (!e.MoveNext())
                    yield break;

                var msg = e.Current;

                Interlocked.Increment(ref m_NumMessagesProcessed);

                if (msg.IsBroken)
                {
                    LogAgent.LogTrace(TaskType, Name, "Broken message propagates through network. Node: {0}, Message: {1}", Name, msg.Title);
                }
                else
                {
                    LogAgent.LogTrace(TaskType, Name, "Processing: {0}", msg.Title);
                }

                yield return msg;
            }
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

                TOutputMsg outputMsg = e.Current;

                if (outputMsg.IsBroken)
                {
                    Interlocked.Increment(ref m_NumBrokenMsgs);
                    LogAgent.LogBrokenMessage(DataflowNetworkConstituent.Transformation, Name, outputMsg.Title, outputMsg);
                }
                else
                {
                    LogAgent.LogTrace(DataflowNetworkConstituent.Transformation, Name, "Transformed: old: {0} new: {1}", outputMsg.Title, outputMsg.Title);
                }

                yield return outputMsg;
            }
        }
    }
}