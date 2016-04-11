using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Decorates the given transformation with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class LogDecoratorTransformation<TIn, TOut> : ITransformationFunctor<TIn, TOut>
    {
        private readonly ITransformationFunctor<TIn, TOut> m_DecoratedTransformation;
        private readonly ILogAgent m_LogAgent;

        private volatile int m_NumMessagesProcessed;
        private volatile int m_NumBrokenMsgs;

        public LogDecoratorTransformation(ITransformationFunctor<TIn, TOut> decoratedTransformation, ILogAgent logAgent)
        {
            m_DecoratedTransformation = decoratedTransformation;
            m_LogAgent = logAgent;
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

        public int NumBrokenMsgs
        {
            get { return m_NumBrokenMsgs; }
        }

        public int NumGoodMsgs
        {
            get { return m_NumMessagesProcessed - m_NumBrokenMsgs; }
        }

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn> item)
        {
            Interlocked.Increment(ref m_NumMessagesProcessed);
            if (item.IsBroken)
            {
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Transformation, Title, "Broken message propagates through network. Node: {0}, Message: {1}", Title, item.Title);
            }
            else
            {
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Transformation, m_DecoratedTransformation.Title, "Transforming: {0}", item.Title);
            }

            IDataflowMessage<TOut> result = m_DecoratedTransformation.Transform(item);

            if (result.IsBroken)
            {
                Interlocked.Increment(ref m_NumBrokenMsgs);
                m_LogAgent.LogBrokenMessage(DataflowNetworkConstituent.Transformation, Title, result.Title, (IBrokenDataFlowMessage<TOut>) result);
            }
            else
            {
                m_LogAgent.LogTrace(DataflowNetworkConstituent.Transformation, m_DecoratedTransformation.Title, "Transformed: old: {0} new: {1}", item.Title, result.Title);
            }
            return result;
        }
    }
}
