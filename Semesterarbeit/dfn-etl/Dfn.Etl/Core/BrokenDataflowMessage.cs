
namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a message that is broken and must be thrown away.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TInput"> </typeparam>
    public sealed class BrokenDataflowMessage<TData> : DefaultDataflowMessage<TData>, IBrokenDataFlowMessage<TData>
    {
        private DataflowNetworkException m_BreakReason;
        private object m_InputContent;

        protected static string GetBrokenTitle(object input)
        {
            return string.Format("[BROKEN] - {0}", input != null ? input.ToString() : string.Empty);
        }

        public BrokenDataflowMessage()
        {
            IsBroken = true;
        }

        public BrokenDataflowMessage(DataflowNetworkException breakReason, object input)
        {
            m_InputContent = input;
            m_BreakReason = breakReason;
            IsBroken = true;
            IsEmpty = true;

            WithTitle(GetBrokenTitle(input));
        }

        public object InputContent
        {
            get { return m_InputContent; }
        }

        public string BreakReason
        {
            get
            {
                return m_BreakReason.Message;
            }
        }

        public DataflowNetworkException BreakException
        {
            get { return m_BreakReason; }
        }

        public override void CloneFrom(IDataflowMessage fromMsg)
        {
            base.CloneFrom(fromMsg);

            var fromBrokenMsg = fromMsg as IBrokenDataFlowMessage<TData>;
            if (fromBrokenMsg == null)
                return;

            m_InputContent = fromBrokenMsg.InputContent;
            m_BreakReason = fromBrokenMsg.BreakException;
        }
    }
}