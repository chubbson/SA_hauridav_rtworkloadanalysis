using System;
using System.Runtime.Serialization;

namespace Dfn.Etl.Core
{
    [Serializable]
    public class DataflowNetworkException : Exception
    {
        private readonly bool m_IsRecoverable;
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DataflowNetworkException(bool recoverable)
        {
            m_IsRecoverable = recoverable;
        }

        public DataflowNetworkException(string message, bool recoverable) : base(message)
        {
            m_IsRecoverable = recoverable;
        }

        public DataflowNetworkException(string message, Exception inner, bool recoverable) : base(message, inner)
        {
            m_IsRecoverable = recoverable;
        }

        protected DataflowNetworkException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public bool IsRecoverable
        {
            get { return m_IsRecoverable; }
        }
    }
}