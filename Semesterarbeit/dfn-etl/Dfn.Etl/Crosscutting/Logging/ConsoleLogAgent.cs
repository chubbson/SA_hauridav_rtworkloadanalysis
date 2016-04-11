using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core;

namespace Dfn.Etl.Crosscutting.Logging
{
    /// <summary>
    /// Represents a log agent that logs its messages to the console.
    /// </summary>
    public sealed class ConsoleLogAgent : ILogAgent
    {
        private readonly LogLevel m_LogLevel;
        private readonly ActionBlock<string> m_InfoMessages;
        private readonly ActionBlock<Tuple<string, Exception>> m_ErrorMessages;

        public ConsoleLogAgent(LogLevel logLevel = LogLevel.Info)
        {
            m_LogLevel = logLevel;
            m_InfoMessages = new ActionBlock<string>(msg => Console.WriteLine(msg));
            m_ErrorMessages = new ActionBlock<Tuple<string, Exception>>(msg => Console.WriteLine(msg.Item1 + " - " + msg.Item2));
        }

        public void LogInfo(string message, params object[] parameter)
        {
            if (m_LogLevel >= LogLevel.Info)
            {
                m_InfoMessages.Post(string.Format(message, parameter));
            }
        }

        public void LogError(string message, Exception exception, params object[] parameter)
        {
            if (m_LogLevel >= LogLevel.Error)
            {
                m_ErrorMessages.Post(new Tuple<string, Exception>(string.Format(message, parameter), exception));
            }
        }

        public void Complete()
        {
            m_InfoMessages.Complete();
            m_ErrorMessages.Complete();
            m_InfoMessages.Completion.Wait();
            m_ErrorMessages.Completion.Wait();
        }

        public void LogTrace(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter)
        {
            if (m_LogLevel >= LogLevel.Trace)
            {
                m_InfoMessages.Post(string.Format("TRACE: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter)));
            }
        }

        public void LogDebug(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter)
        {
            if (m_LogLevel >= LogLevel.Debug)
            {
                m_InfoMessages.Post(string.Format("DEBUG: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter)));
            }
        }

        public void LogInfo(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter)
        {
            if (m_LogLevel >= LogLevel.Info)
            {
                m_InfoMessages.Post(string.Format("INFO: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter)));
            }
        }

        public void LogError(DataflowNetworkConstituent networkConstituent, string title, Exception exception, string messageFormat, params object[] parameter)
        {
            if (m_LogLevel >= LogLevel.Error)
            {
                m_ErrorMessages.Post(new Tuple<string, Exception>(string.Format("ERROR: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter)), exception));
            }
        }

        public void LogFatal(DataflowNetworkConstituent networkConstituent, string title, Exception exception)
        {
            m_ErrorMessages.Post(new Tuple<string, Exception>(string.Format("FATAL: [{0}] Name: {1} - FATAL NETWORK ERROR", networkConstituent, title), exception));
        }

        public void LogUnknown(DataflowNetworkConstituent networkConstituent, string title, Exception exception)
        {
            if (m_LogLevel >= LogLevel.Error)
            {
                m_ErrorMessages.Post(new Tuple<string, Exception>(string.Format("ERROR: [{0}] Name: {1} - UNKNOWN NETWORK ERROR", networkConstituent, title), exception));
            }
        }

        public void LogBrokenMessage<TContent>(DataflowNetworkConstituent networkConstituent, string title, string messageTitle, IBrokenDataFlowMessage<TContent> broken)
        {
            if (m_LogLevel >= LogLevel.Debug)
            {
                m_ErrorMessages.Post(new Tuple<string, Exception>(string.Format("DEBUG: [{0}] Name: {1} - Message: {2}", networkConstituent, title, messageTitle), broken.BreakException));
            }
            else if (m_LogLevel >= LogLevel.Info)
            {
                m_InfoMessages.Post(string.Format("INFO: [{0}-BROKEN-MSG] Name: {1} - Message-Name: {2} - Break Reason: {3}", networkConstituent, title, messageTitle, broken.BreakReason));
            }
        }

        public void LogBrokenMessage<TMsg>(DataflowNetworkConstituent networkConstituent, string title, string messageTitle, TMsg broken)
            where TMsg : IDataflowMessage
        {
            var errors = broken.Errors.ToList();
            if (errors.Count == 0)
            {
                return;
            }

            Exception ex = null;
            ErrorMessage m = null;
            if (errors.Count == 1)
            {
                ex = errors[0].Exception;
                m = errors[0].Message;
            }
            else
            {
                var exceptions = errors.Select(error => error.Exception).ToList();
                if (exceptions.Count == 1)
                {
                    ex = exceptions[0];
                }
                else if (exceptions.Count > 1)
                {
                    ex = new AggregateException(exceptions);
                }
                m = new AggregateErrorMessage(errors.Select(error => error.Message));
            }

            if (m_LogLevel >= LogLevel.Debug)
            {
                m_ErrorMessages.Post(new Tuple<string, Exception>(string.Format("DEBUG: [{0}] Name: {1} - Message: {2}", networkConstituent, title, messageTitle), ex));
            }
            else if (m_LogLevel >= LogLevel.Info)
            {
                m_InfoMessages.Post(string.Format("INFO: [{0}-BROKEN-MSG] Name: {1} - Message-Name: {2} - Break Reason: {3}", networkConstituent, title, messageTitle, m));
            }
        }
    }
}