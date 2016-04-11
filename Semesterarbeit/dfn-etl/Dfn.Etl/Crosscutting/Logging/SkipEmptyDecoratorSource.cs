using System.Collections.Generic;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Crosscutting.Logging
{
    public sealed class SkipEmptyDecoratorSource<TOut> : ISourceFunctor<TOut>
    {
        private readonly ISourceFunctor<TOut> m_DecoratedSource;
        
        public SkipEmptyDecoratorSource(ISourceFunctor<TOut> decoratedSource)
        {
            m_DecoratedSource = decoratedSource;
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
            foreach (var dataflowMessage in m_DecoratedSource.Pull())
            {
                if (dataflowMessage == null || dataflowMessage.IsEmpty)
                {
                    continue;
                }

                yield return dataflowMessage;
            }
        }
    }
}