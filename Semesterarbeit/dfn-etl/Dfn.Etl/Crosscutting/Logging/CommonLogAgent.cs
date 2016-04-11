using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Common.Logging;
using Dfn.Etl.Core;
using Dfn.Etl.Crosscutting.WorkloadStatistics;
using NetMQ;

namespace Dfn.Etl.Crosscutting.Logging
{
    public sealed class CommonLogAgent : ILogAgent
    {
        private readonly ILog m_Log;
        private readonly ActionBlock<LogLevelMessageException> m_Messages;
        private readonly NetMQSocket m_ClientSocket;

        public CommonLogAgent(ILog log)
        {
            m_Log = log;
            m_Messages = new ActionBlock<LogLevelMessageException>(e => Log(e));

            var pub = new WorkloadStatisticsZmqPublisher();
            pub.Init();
            m_ClientSocket = pub.GetPubSocket();
        
        }

        public void Complete()
        {
            m_Messages.Complete();
            m_Messages.Completion.Wait();
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
            m_Messages.Post(new LogLevelMessageException(LogLevel.Debug, string.Format("DEBUG: [{0}] Name: {1} - Message: {2}", networkConstituent, title, messageTitle), ex));
            m_Messages.Post(new LogLevelMessageException(LogLevel.Info, string.Format("INFO: [{0}-BROKEN-MSG] Name: {1} - Message-Name: {2} - Break Reason: {3}", networkConstituent, title, messageTitle, m)));
        }

        public void LogBrokenMessage<TContent>(DataflowNetworkConstituent networkConstituent, string title, string messageTitle, IBrokenDataFlowMessage<TContent> broken)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Debug, string.Format("DEBUG: [{0}] Name: {1} - Message: {2}", networkConstituent, title, messageTitle), broken.BreakException));
            m_Messages.Post(new LogLevelMessageException(LogLevel.Info, string.Format("INFO: [{0}-BROKEN-MSG] Name: {1} - Message-Name: {2} - Break Reason: {3}", networkConstituent, title, messageTitle, broken.BreakReason)));
        }

        public void LogDebug(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Debug, string.Format("DEBUG: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter))));
        }

        public void LogError(string message, Exception exception, params object[] parameter)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Error, string.Format(message, parameter)));
        }

        public void LogError(DataflowNetworkConstituent networkConstituent, string title, Exception exception, string messageFormat, params object[] parameter)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Error, string.Format("ERROR: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter)), exception));
        }

        public void LogFatal(DataflowNetworkConstituent networkConstituent, string title, Exception exception)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Fatal, string.Format("FATAL: [{0}] Name: {1} - FATAL NETWORK ERROR", networkConstituent, title), exception));
        }

        public void LogInfo(string message, params object[] parameter)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Info, string.Format(message, parameter)));
        }

        public void LogInfo(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Info, string.Format("INFO: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter))));
        }

        public void LogTrace(DataflowNetworkConstituent networkConstituent, string title, string messageFormat, params object[] parameter)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Trace, string.Format("TRACE: [{0}] Name: {1} - Message: {2}", networkConstituent, title, string.Format(messageFormat, parameter))));
        }

        public void LogUnknown(DataflowNetworkConstituent networkConstituent, string title, Exception exception)
        {
            m_Messages.Post(new LogLevelMessageException(LogLevel.Error, string.Format("ERROR: [{0}] Name: {1} - UNKNOWN NETWORK ERROR", networkConstituent, title), exception));
        }

        private void Log(LogLevelMessageException message)
        {

            var bc = -1; //GetBoundedCapacity();
            var nms = -1;

            var m = string.Format("NMS: {0}|BC: {1}|IncMsg: {2}|OutMsg: {3}|Title: {4}", nms, bc, m_Messages.InputCount, -1, message.Message);
            //m_ClientSocket.Send(m);
            m_ClientSocket.Send(m);
            /*
            switch (message.Level)
            {
                case LogLevel.Info:
                    m_Log.Info(message.Message, message.Exception);
                    break;
                case LogLevel.Error:
                    m_Log.Error(message.Message, message.Exception);
                    break;
                case LogLevel.Fatal:
                    m_Log.Fatal(message.Message, message.Exception);
                    break;
                case LogLevel.Debug:
                    m_Log.Debug(message.Message, message.Exception);
                    break;
                case LogLevel.Trace:
                    m_Log.Trace(message.Message, message.Exception);
                    m_Log.Debug(message.Message, message.Exception);
                    break;
            }
            */
            message = null;
        }

        private sealed class LogLevelMessageException
        {
            private readonly Exception m_Exception;
            private readonly LogLevel m_LogLevel;
            private readonly string m_Message;

            public LogLevelMessageException(LogLevel logLevel, String message, Exception exception = null)
            {
                m_LogLevel = logLevel;
                m_Message = message;
                m_Exception = exception;
            }

            public Exception Exception
            {
                get
                {
                    return m_Exception;
                }
            }

            public LogLevel Level
            {
                get
                {
                    return m_LogLevel;
                }
            }

            public string Message
            {
                get
                {
                    return m_Message;
                }
            }
        }
    }
}