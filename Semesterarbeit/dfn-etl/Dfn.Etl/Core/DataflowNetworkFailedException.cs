using System;
using System.Runtime.Serialization;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents an exception that is thrown when an entire dataflow-network has not completed successfully.
    /// </summary>
    [Serializable]
    public sealed class DataflowNetworkFailedException : DataflowNetworkException
    {
        private const bool UNRECOVERABLE = false;

        public DataflowNetworkFailedException() : base(UNRECOVERABLE)
        {
        }

        public DataflowNetworkFailedException(string message)
            : base(message, UNRECOVERABLE)
        {
        }

        public DataflowNetworkFailedException(string message, Exception inner)
            : base(message, inner, UNRECOVERABLE)
        {
        }

        private DataflowNetworkFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
