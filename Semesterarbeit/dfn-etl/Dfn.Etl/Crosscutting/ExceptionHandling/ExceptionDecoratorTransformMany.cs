using System.Collections.Generic;
using System.Linq;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents a decorator that enhances a one-to-many transformation with exception handling facilities.
    /// </summary>
    public sealed class ExceptionDecoratorTransformMany<TIn, TOut> : ITransformManyFunctor<TIn, TOut>
    {
        private readonly ITransformMany<TIn, TOut> m_Decorated;
        private readonly ILogAgent m_LogAgent;
        private readonly ICancelNetwork m_Cancel;

        public ExceptionDecoratorTransformMany(ITransformMany<TIn,TOut> decorated, ILogAgent logAgent, ICancelNetwork cancel)
        {
            m_Decorated = decorated;
            m_LogAgent = logAgent;
            m_Cancel = cancel;
        }

        public string Title
        {
            get { return m_Decorated.Title; }
        }

        public IEnumerable<IDataflowMessage<TOut>> TransformMany(IDataflowMessage<TIn> item)
        {
            try
            {
                if (item.IsBroken)
                {
                    // Kind of unexpected, as those guys should usually be filtered
                    return new List<IDataflowMessage<TOut>>
                               {
                                   new BrokenDataflowMessage<TOut>(null, item.Data)
                               };
                }
                var result = m_Decorated.TransformMany(item.Data).Select(c => new DefaultDataflowMessage<TOut>(c).WithTitle(Title));
                return result;
            }
            catch (DataflowNetworkRecoverableErrorException recoverableError)
            {
                return new List<IDataflowMessage<TOut>>
                           {
                               new BrokenDataflowMessage<TOut>(recoverableError, item.Data)
                           };
                
            }
            catch(DataflowNetworkUnrecoverableErrorException unrecoverableError)
            {
                // bring the network down.
                m_LogAgent.LogFatal(DataflowNetworkConstituent.TransformMany, Title, unrecoverableError);
                m_Cancel.CancelNetwork();
                return new List<IDataflowMessage<TOut>>
                           {
                               new BrokenDataflowMessage<TOut>(unrecoverableError, item.Data)
                           };
            }
        }
    }
}