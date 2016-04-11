namespace Dfn.Etl.Core.Functors
{
    public interface ITransformationBatchedFunctor<in TIn, out TOut>
    {
        string Title { get; }
        IDataflowMessage<TOut> Transform(IDataflowMessage<TIn>[] item); 
    }
}