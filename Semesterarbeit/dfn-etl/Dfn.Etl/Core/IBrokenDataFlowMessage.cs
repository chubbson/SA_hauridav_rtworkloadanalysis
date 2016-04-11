namespace Dfn.Etl.Core
{
    public interface IBrokenDataFlowMessage<out TContent> : IDataflowMessage<TContent>
    {
        /// <summary>
        /// The reason why this message is broken.
        /// </summary>
        string BreakReason { get; }

        /// <summary>
        /// Returns the exception that led to the message being broken.
        /// </summary>
        DataflowNetworkException BreakException { get; }

        /// <summary>
        /// Returns the input content (if any).
        /// </summary>
        object InputContent { get; }
    }
}