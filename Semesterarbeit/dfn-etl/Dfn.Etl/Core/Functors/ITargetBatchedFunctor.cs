namespace Dfn.Etl.Core.Functors
{
    public interface ITargetBatchedFunctor<in TIn>
    {
        string Title { get; }
        void Push(IDataflowMessage<TIn>[] items); 
    }
}