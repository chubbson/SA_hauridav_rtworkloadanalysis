using System.Linq;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents a decorator that enhances a batched target with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class ExceptionDecoratorTargetBatched<TIn> : ITargetBatchedFunctor<TIn>
    {
        private readonly ITargetBatched<TIn> m_DecoratedTarget;
        private readonly ILogAgent m_LogAgent;
        private readonly ICancelNetwork m_Cancel;

        public ExceptionDecoratorTargetBatched(ITargetBatched<TIn> decoratedTarget, ILogAgent logAgent, ICancelNetwork cancel)
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

        public void Push(IDataflowMessage<TIn>[] items)
        {
            try
            {
                m_DecoratedTarget.Push(items.Where(item => !item.IsBroken).Select(item => item.Data).ToArray());
            }
            catch (DataflowNetworkUnrecoverableErrorException ex)
            {
                m_LogAgent.LogFatal(DataflowNetworkConstituent.TargetBatched, m_DecoratedTarget.Title, ex);
                m_Cancel.CancelNetwork();
            }
            catch (DataflowNetworkRecoverableErrorException ex)
            {
                m_LogAgent.LogUnknown(DataflowNetworkConstituent.TargetBatched, m_DecoratedTarget.Title, ex);
            }
        }
    }
}
