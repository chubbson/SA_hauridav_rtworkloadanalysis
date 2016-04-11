using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    public class SkipEmptyDecoratorTransformation<TIn, TOut> : ITransformationFunctor<TIn, TOut>
    {
        private readonly ITransformationFunctor<TIn, TOut> m_DecoratedTransformation;
        
        private volatile int m_NumMessagesProcessed;
        private volatile int m_NumEmptyMessages;

        public SkipEmptyDecoratorTransformation (ITransformationFunctor<TIn, TOut> decoratedTransformation)
        {
            m_DecoratedTransformation = decoratedTransformation;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTransformation.Title;
            }
        }

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

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn> item)
        {
            Interlocked.Increment(ref m_NumMessagesProcessed);
            IDataflowMessage<TOut> result = m_DecoratedTransformation.Transform(item);

            if (result.IsEmpty)
            {
                Interlocked.Increment(ref m_NumEmptyMessages);
            }
            
            return result;
        }
    }
}