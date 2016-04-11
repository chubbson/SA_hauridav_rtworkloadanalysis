using System.Collections.Generic;
using System.Linq;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents a decorator that enhances a given source with exception handling facilities.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public class ExceptionDecoratorSource<TOut> : ISourceFunctor<TOut>
    {
        private readonly ISource<TOut> m_DecoratedSource;
        private readonly ILogAgent m_LogAgent;
        private readonly ICancelNetwork m_Cancel;

        public ExceptionDecoratorSource(ISource<TOut> decoratedSource, ILogAgent logAgent, ICancelNetwork cancel)
        {
            m_DecoratedSource = decoratedSource;
            m_LogAgent = logAgent;
            m_Cancel = cancel;
        }

        public string Title
        {
            get
            {
                return m_DecoratedSource.Title;
            }
        }

        public IEnumerable<IDataflowMessage<TOut>> Pull()
        {
            // Unfortunately, this guy can't do much, because he will either have to enumerate the enumerable
            // (thereby materializing it) or just delay the computation further as is the case here.
            return m_DecoratedSource.Pull().Select(item => new DefaultDataflowMessage<TOut>(item).WithTitle(Title));
        }

        /*public IEnumerable<IDataflowMessage<TOut>> Pull()
        {
            // This approach is necessary to catch exceptions produced by a single item returned by the IEnumerable. Justifications:
            // 1) C# does not allow try-catch blocks for yield return statements.
            // 2) GetEnumerator() throws the exception that is why it is inside the while-loop.
            // See: http://blogs.msdn.com/b/ericlippert/archive/2009/07/20/iterator-blocks-part-four-why-no-yield-in-catch.aspx
            IEnumerator<TOut> sourceEnumerator = null;
            while (true)
            {
                IDataflowMessage<TOut> wrappedResult;
                try
                {
                    if (sourceEnumerator == null)
                    {
                        sourceEnumerator = m_DecoratedSource.Pull().GetEnumerator();
                    }
                    if (!sourceEnumerator.MoveNext())
                    {
                        yield break;
                    }
                    TOut result = sourceEnumerator.Current;
                    wrappedResult = new DefaultDataflowMessage<TOut>(Title, result);
                }
                catch (DataflowNetworkUnrecoverableErrorException ex)
                {
                    m_LogAgent.LogFatal(DataflowNetworkConstituent.Source, m_DecoratedSource.Title, ex);
                    m_Cancel.CancelNetwork();
                    wrappedResult = new BrokenDataflowMessage<TOut,TOut>(Title, "Dataflow-Network has been cancelled.", default(TOut));
                }
                catch (DataflowNetworkRecoverableErrorException ex)
                {
                    m_LogAgent.LogUnknown(DataflowNetworkConstituent.Source, m_DecoratedSource.Title, ex);
                    wrappedResult = new BrokenDataflowMessage<TOut, TOut>(Title, "Item broke.", default(TOut));
                }
                
                yield return wrappedResult;
            }
        }*/
    }
}
