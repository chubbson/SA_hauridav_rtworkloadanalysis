using System.Collections.Generic;

namespace Dfn.Etl.Core
{
    public interface ITransformMany<in TIn, out TOut>
    {
        string Title { get; }
        IEnumerable<TOut> TransformMany(TIn item);
    }
}