using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents a decorator that enhances a target with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class ExceptionDecoratorTarget<TIn> : ITargetFunctor<TIn>
    {
        private readonly ITarget<TIn> m_DecoratedTarget;
        private readonly ILogAgent m_LogAgent;
        private readonly ICancelNetwork m_Cancel;

        public ExceptionDecoratorTarget(ITarget<TIn> decoratedTarget, ILogAgent logAgent, ICancelNetwork cancel)
        {
            m_DecoratedTarget = decoratedTarget;
            m_LogAgent = logAgent;
            m_Cancel = cancel;
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
            try
            {
                if (!item.IsBroken)
                {
                    m_DecoratedTarget.Push(item.Data);
                }
            }
            catch (DataflowNetworkUnrecoverableErrorException ex)
            {
                m_LogAgent.LogFatal(DataflowNetworkConstituent.Target, m_DecoratedTarget.Title, ex);
                // Halt the entire dataflow network.
                m_Cancel.CancelNetwork();
            }
            catch (DataflowNetworkRecoverableErrorException ex)
            {
                var broken = new BrokenDataflowMessage<TIn>(ex, item.Data);
                m_LogAgent.LogBrokenMessage(DataflowNetworkConstituent.Target, Title, item.Title, broken);
            }
        }
    }
}
