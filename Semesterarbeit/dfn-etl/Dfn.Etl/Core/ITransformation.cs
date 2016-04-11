namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a constituent of a dataflow network that transforms an item of TIn into an item of TOut.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public interface ITransformation<in TIn, out TOut>
    {
        string Title { get; }
        TOut Transform(TIn item);
    }

    public interface IContentTransformation<in TIn, out TOut> : ITransformation<IDataflowMessage<TIn>, IDataflowMessage<TOut>>
    {
    }
}