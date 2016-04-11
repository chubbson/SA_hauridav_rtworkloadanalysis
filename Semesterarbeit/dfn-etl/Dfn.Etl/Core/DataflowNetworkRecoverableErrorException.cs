using System;
using System.Runtime.Serialization;

namespace Dfn.Etl.Core
{
    [Serializable]
    public sealed class DataflowNetworkRecoverableErrorException : DataflowNetworkException
    {
        private const bool RECOVERABLE = true;
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DataflowNetworkRecoverableErrorException() : base(RECOVERABLE)
        {
        }

        public DataflowNetworkRecoverableErrorException(string message) : base(message, RECOVERABLE)
        {
        }

        public DataflowNetworkRecoverableErrorException(string message, Exception inner) : base(message, inner, RECOVERABLE)
        {
        }

        private DataflowNetworkRecoverableErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}