using System.Collections.Generic;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Decorates the given one-to-many transformation with logging capabilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class LogDecoratorTransformMany<TIn, TOut> : ITransformManyFunctor<TIn, TOut>
    {
        private readonly ITransformManyFunctor<TIn, TOut> m_Decorated;
        private readonly ILogAgent m_LogAgent;

        public LogDecoratorTransformMany(ITransformManyFunctor<TIn, TOut> decorated, ILogAgent logAgent)
        {
            m_Decorated = decorated;
            m_LogAgent = logAgent;
        }

        public string Title
        {
            get { return m_Decorated.Title; }
        }

        public IEnumerable<IDataflowMessage<TOut>> TransformMany(IDataflowMessage<TIn> item)
        {
            m_LogAgent.LogTrace(DataflowNetworkConstituent.TransformMany, m_Decorated.Title, "Transforming: {0}", item.Title);
            var result = m_Decorated.TransformMany(item);
            m_LogAgent.LogTrace(DataflowNetworkConstituent.TransformMany, m_Decorated.Title, "Transformed: {0}", item.Title);
            return result;
        }
    }
}