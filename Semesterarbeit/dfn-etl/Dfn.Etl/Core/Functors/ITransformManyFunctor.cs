using System.Collections.Generic;

namespace Dfn.Etl.Core.Functors
{
    public interface ITransformManyFunctor<in TIn, out TOut>
    {
        string Title { get; }
        IEnumerable<IDataflowMessage<TOut>> TransformMany(IDataflowMessage<TIn> item);
    }
}
    