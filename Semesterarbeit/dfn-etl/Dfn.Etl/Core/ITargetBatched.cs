namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a target that receives a group of items.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public interface ITargetBatched<in TIn>
    {
        string Title { get; }
        void Push(TIn[] items);
    }
}