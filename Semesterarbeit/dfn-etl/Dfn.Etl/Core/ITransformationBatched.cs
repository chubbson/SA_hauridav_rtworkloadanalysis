namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a constituent of a dataflow network that transforms batched items TIn into an item of TOut.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public interface ITransformationBatched<in TIn, out TOut>
    {
        string Title { get; }
        TOut Transform(TIn[] item);
    }
}