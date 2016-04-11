using System.Linq;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents a decorator that enhances a many-to-one transformation with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class ExceptionDecoratorTransformationBatched<TIn, TOut> : ITransformationBatchedFunctor<TIn, TOut>
    {
        private readonly ITransformationBatched<TIn, TOut> m_DecoratedTransformation;
        private readonly ILogAgent m_LogAgent;
        private readonly ICancelNetwork m_Cancel;

        public ExceptionDecoratorTransformationBatched(ITransformationBatched<TIn, TOut> decoratedTransformation, ILogAgent logAgent, ICancelNetwork cancel)
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

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn>[] items)
        {
            // skipped broken messages - logDecoratorTransformBatched logs count of broken messages and their titles. 
            var skippedBrokenItems = items.Where(item => !item.IsBroken).Select(item => item.Data).ToArray();

            try
            {
                if (!skippedBrokenItems.Any())
                {
                    return new BrokenDataflowMessage<TOut>(
                                new DataflowNetworkRecoverableErrorException(string.Format("All messages in batch are broken, Batch-Size: {0}", items.Count()))
                                , skippedBrokenItems);
                }

                TOut result = m_DecoratedTransformation.Transform(skippedBrokenItems);

                return new DefaultDataflowMessage<TOut>(result).WithTitle(Title);
            }
            catch (DataflowNetworkUnrecoverableErrorException ex)
            {
                m_LogAgent.LogFatal(DataflowNetworkConstituent.TransformationBatched, m_DecoratedTransformation.Title, ex);
                m_Cancel.CancelNetwork();

                return new BrokenDataflowMessage<TOut>(ex, skippedBrokenItems);
            }
            catch (DataflowNetworkRecoverableErrorException ex)
            { 
                return new BrokenDataflowMessage<TOut>(ex, skippedBrokenItems);
            }
        }
    }
}
