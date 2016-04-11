using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents a decorator that enhances a transformation with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class ExceptionDecoratorTransformation<TIn, TOut> : ITransformationFunctor<TIn, TOut>
    {
        private readonly ITransformation<TIn, TOut> m_DecoratedTransformation;
        private readonly ILogAgent m_LogAgent;
        private readonly ICancelNetwork m_Cancel;

        public ExceptionDecoratorTransformation(ITransformation<TIn, TOut> decoratedTransformation, ILogAgent logAgent, ICancelNetwork cancel)
        {
            m_DecoratedTransformation = decoratedTransformation;
            m_LogAgent = logAgent;
            m_Cancel = cancel;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTransformation.Title;
            }
        }

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn> item)
        {
            try
            {
                if (item.IsBroken)
                {
                    var broken = (IBrokenDataFlowMessage<TIn>)item;
                    return new BrokenDataflowMessage<TOut>(broken.BreakException,item.Data);
                }

                TOut result = m_DecoratedTransformation.Transform(item.Data);
                return new DefaultDataflowMessage<TOut>(result).WithTitle(Title);
            }
            catch (DataflowNetworkUnrecoverableErrorException ex)
            {
                m_LogAgent.LogFatal(DataflowNetworkConstituent.Transformation, m_DecoratedTransformation.Title, ex);
                m_Cancel.CancelNetwork();
                return new BrokenDataflowMessage<TOut>(ex, item.Data);
            }
            catch (DataflowNetworkRecoverableErrorException ex)
            {
                return new BrokenDataflowMessage<TOut>(ex, item.Data);
            }
        }
    }
}
