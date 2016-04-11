using System.Collections.Generic;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Decorates the given source with logging capabilities.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public sealed class LogDecoratorSource<TOut> : ISourceFunctor<TOut>
    {
        private readonly ISourceFunctor<TOut> m_DecoratedSource;
        private readonly ILogAgent m_LogAgent;

        private volatile int m_NumMessagesProcessed;
        private volatile int m_NumBrokenMsgs;

        public LogDecoratorSource(ISourceFunctor<TOut> decoratedSource, ILogAgent logAgent)
        {
            m_DecoratedSource = decoratedSource;
            m_LogAgent = logAgent;
        }

        public string Title
        {
            get
            {
                return m_DecoratedSource.Title;
            }
        }

        public int NumMessagesSeen
        {
            get { return m_NumMessagesProcessed; }
        }
        
        public IEnumerable<IDataflowMessage<TOut>> Pull()
        {
            m_LogAgent.LogInfo(DataflowNetworkConstituent.Source, m_DecoratedSource.Title, "Pulling started.");
            foreach (var dfMsg in m_DecoratedSource.Pull())
            {
                Interlocked.Increment(ref m_NumMessagesProcessed);
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Source, m_DecoratedSource.Title, "Pulled source: {0}", dfMsg.Title);
                yield return dfMsg;
            }
            m_LogAgent.LogInfo(DataflowNetworkConstituent.Source, m_DecoratedSource.Title, "Pulling finished.");
        }
    }
}
