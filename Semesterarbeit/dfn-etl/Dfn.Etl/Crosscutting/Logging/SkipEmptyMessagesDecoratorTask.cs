﻿using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Crosscutting.Logging
{
    public class SkipEmptyMessagesDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> : DataflowNetworkDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>
        where TOutputMsg : IDataflowMessage<TOutput>
        where TInputMsg : IDataflowMessage<TInput>
    {
        private volatile int m_NumMessagesProcessed;
        private volatile int m_NumEmptyMessages;

        public int NumMessagesSeen
        {
            get { return m_NumMessagesProcessed; }
        }

        public int NumEmptyMessages
        {
            get { return m_NumEmptyMessages; }
        }

        public int NumNonEmptyMessages
        {
            get { return m_NumMessagesProcessed - m_NumEmptyMessages; }
        }

        public SkipEmptyMessagesDecoratorTask(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> innerTask, IIdasDataflowNetwork network, ILogAgent logAgent)
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
                if(!e.MoveNext())
                    yield break;

                //Get the next output message
                TOutputMsg outputMsg = e.Current;

                Interlocked.Increment(ref m_NumMessagesProcessed);

                if (outputMsg.IsEmpty)
                {
                    Interlocked.Increment(ref m_NumEmptyMessages);
                }

                yield return outputMsg;
            }
        }
    }
}