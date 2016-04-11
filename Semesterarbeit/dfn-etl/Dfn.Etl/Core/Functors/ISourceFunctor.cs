using System.Collections.Generic;

namespace Dfn.Etl.Core.Functors
{
    public interface ISourceFunctor<out TOut>
    {
        string Title { get; }
        IEnumerable<IDataflowMessage<TOut>> Pull();
    }
}