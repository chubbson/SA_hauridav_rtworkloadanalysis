using System;
using Dfn.Etl.Core;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Represents an agent that logs messages independently on a separate thread.
    /// </summary>
    public interface ILogAgent
    {
        void LogTrace(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter);
        void LogDebug(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter);
        void LogInfo(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter);
        void LogError(DataflowNetworkConstituent networkConstituent, string title, Exception exception, string messageFormat, params object[] parameter);
        void LogFatal(DataflowNetworkConstituent networkConstituent, string title, Exception exception);
        void LogUnknown(DataflowNetworkConstituent networkConstituent, string title, Exception exception);
        void LogBrokenMessage<TContent>(DataflowNetworkConstituent networkConstituent, string title, string messageTitle, IBrokenDataFlowMessage<TContent> message);
        void LogBrokenMessage<TMsg>(DataflowNetworkConstituent networkConstituent, string title, string messageTitle, TMsg message) where TMsg : IDataflowMessage;

        /// <summary>
        /// Tells this agent to stop its operations.
        /// </summary>
        void Complete();
    }
}