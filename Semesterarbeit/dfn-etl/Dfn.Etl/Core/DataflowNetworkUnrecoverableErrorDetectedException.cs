using System;
using System.Runtime.Serialization;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents an exception that is thrown when a part of the dataflow-network has detected an error that is irreparable and therefore
    /// must result in the entire dataflow-network to be aborted.
    /// Note: Throw this exception in your dataflow-network objects to signal the network that it should halt itself.
    /// </summary>
    [Serializable]
    public sealed class DataflowNetworkUnrecoverableErrorException : DataflowNetworkException
    {
        private const bool UNRECOVERABLE = false;

        public DataflowNetworkUnrecoverableErrorException() : base(UNRECOVERABLE)
        {
        }

        public DataflowNetworkUnrecoverableErrorException(string message)
            : base(message, UNRECOVERABLE)
        {
        }

        public DataflowNetworkUnrecoverableErrorException(string message, Exception inner)
            : base(message, inner, UNRECOVERABLE)
        {
        }

        private DataflowNetworkUnrecoverableErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}