using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Decorates the given target with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class LogDecoratorTarget<TIn> : ITargetFunctor<TIn>
    {
        private readonly ITargetFunctor<TIn> m_DecoratedTarget;
        private readonly ILogAgent m_LogAgent;
        
        public LogDecoratorTarget(ITargetFunctor<TIn> decoratedTarget, ILogAgent logAgent)
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

        public void Push(IDataflowMessage<TIn> item)
        {
            if (!item.IsBroken)
            {
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Target, m_DecoratedTarget.Title, "Pushing: {0}", item.Title);
                m_DecoratedTarget.Push(item);
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Target, m_DecoratedTarget.Title, "Pushed: {0}", item.Title);
            }
            else
            {
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Target, Title, "Broken message propagated through network. Title: {0}", item.Title);
            }
        }
    }
}