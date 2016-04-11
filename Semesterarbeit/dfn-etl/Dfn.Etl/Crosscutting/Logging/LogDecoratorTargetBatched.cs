using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Decorates the given batched target with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class LogDecoratorTargetBatched<TIn> : ITargetBatchedFunctor<TIn>
    {
        private readonly ITargetBatchedFunctor<TIn> m_DecoratedTarget;
        private readonly ILogAgent m_LogAgent;

        public LogDecoratorTargetBatched(ITargetBatchedFunctor<TIn> decoratedTarget, ILogAgent logAgent)
        {
            m_DecoratedTarget = decoratedTarget;
            m_LogAgent = logAgent;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTarget.Title;
            }
        }

        public void Push(IDataflowMessage<TIn>[] items)
        {
            m_LogAgent.LogTrace(DataflowNetworkConstituent.TargetBatched, m_DecoratedTarget.Title, "Pushing with Batch-Size: {0}", items.Length);
            foreach (IDataflowMessage<TIn> item in items)
            {
                if (!item.IsBroken)
                {
                    m_LogAgent.LogTrace(DataflowNetworkConstituent.TargetBatched, m_DecoratedTarget.Title, "Pushing: {0}", item.Title);
                }
                else
                {
                    m_LogAgent.LogTrace(DataflowNetworkConstituent.Target, Title, "Broken message propagated through network. Title: {0}", item.Title);
                }
            }
            m_DecoratedTarget.Push(items);
            foreach (IDataflowMessage<TIn> item in items)
            {
                m_LogAgent.LogTrace(DataflowNetworkConstituent.TargetBatched, m_DecoratedTarget.Title, "Pushed: {0}", item.Title);
            }
            m_LogAgent.LogTrace(DataflowNetworkConstituent.TargetBatched, m_DecoratedTarget.Title, "Pushed with Batch-Size: {0}", items.Length);
        }
    }
}
