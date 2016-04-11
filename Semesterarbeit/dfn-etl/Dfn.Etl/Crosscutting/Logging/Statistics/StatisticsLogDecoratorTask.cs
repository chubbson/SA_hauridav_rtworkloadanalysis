using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    public class StatisticsLogDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> 
        : DataflowNetworkDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>, IStatisticsLogger
        where TOutputMsg : IDataflowMessage<TOutput>
        where TInputMsg : IDataflowMessage<TInput>
    {
        private long m_NumMessagesProcessed;
        private long m_NumBrokenMsgs;

        public long NumProcessedMessages
        {
            get { return m_NumMessagesProcessed; }
        }

        public long NumBrokenMsgs
        {
            get { return m_NumBrokenMsgs; }
        }

        public long NumGoodMsgs
        {
            get { return m_NumMessagesProcessed - m_NumBrokenMsgs; }
        }

        public StatisticsLogDecoratorTask(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> innerTask, IIdasDataflowNetwork network, ILogAgent logAgent)
            : base(innerTask, network, logAgent)
        {
        }

        public void LogStatistics()
        {
            string stats = StatisticsHelper.FormatLogMessage(Name, "", Interlocked.Read(ref m_NumMessagesProcessed), Interlocked.Read(ref m_NumBrokenMsgs));
            LogAgent.LogInfo(TaskType, Name, stats);
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

                Interlocked.Increment(ref m_NumMessagesProcessed);

                TOutputMsg outputMsg = e.Current;

                if (outputMsg.IsBroken)
                {
                    Interlocked.Increment(ref m_NumBrokenMsgs);
                }

                yield return outputMsg;
            }
        }
    }
}