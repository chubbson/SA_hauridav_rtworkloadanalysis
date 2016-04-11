using System.Threading;
using Dfn.Etl.Core;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    public abstract class StatisticsLoggerBase : IStatisticsLogger
    {
        
        protected long NumBrokenMessages;
        protected long NumProcessedMessages;

        private readonly ILogAgent m_LogAgent;
        private readonly string m_Title;
        private readonly DataflowNetworkConstituent m_Constituent;

        protected StatisticsLoggerBase(ILogAgent logAgent, string title, DataflowNetworkConstituent constituent)
        {
            m_LogAgent = logAgent;
            m_Title = title;
            m_Constituent = constituent;
        }

        public void LogStatistics()
        {
            string dfnConstituentType = m_Constituent.ToString("G");
            string stats = StatisticsHelper.FormatLogMessage(m_Title, dfnConstituentType, Interlocked.Read(ref NumProcessedMessages), Interlocked.Read(ref NumBrokenMessages));
            m_LogAgent.LogInfo(m_Constituent, m_Title, stats);
        }
    }
}