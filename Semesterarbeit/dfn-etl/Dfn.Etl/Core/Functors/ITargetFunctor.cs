namespace Dfn.Etl.Core.Functors
{
    public interface ITargetFunctor<in TIn>
    {
        string Title { get; }
        void Push(IDataflowMessage<TIn> item);
    }
}