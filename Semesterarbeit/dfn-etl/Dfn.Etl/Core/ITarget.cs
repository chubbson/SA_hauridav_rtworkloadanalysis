namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a target of a dataflow network that receives one input. 
    /// </summary>
    /// <typeparam name="TIn"></typeparam> 
    public interface ITarget<in TIn>
    {
        string Title { get; }
        void Push(TIn item);
    }

}