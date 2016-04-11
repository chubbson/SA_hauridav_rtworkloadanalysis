using System;

namespace Dfn.Etl.Crosscutting.Logging.Statistics
{
    internal static class StatisticsHelper
    {
        private const string LOG_STRING =
            "{0}{1}{0}" +
            "{2}{3}{0}" +
            "{10}{11}{0}" +
            "{4}{5}{0}" +
            "{6}{7}{0}" +
            "{8}{9}{0}" +
            "{1}{0}";

        internal const long UNKNOWN = -1L;
        private const string NA = "n/a";

        internal static string FormatLogMessage(string name, string constituent, long numMessagesProcessed, long numMessagesBroken)
        {
// ReSharper disable SpecifyACultureInStringConversionExplicitly
            string numBroken = numMessagesBroken <= UNKNOWN ? NA : numMessagesBroken.ToString();
            string numGood = numMessagesBroken <= UNKNOWN ? NA : (numMessagesProcessed - numMessagesBroken).ToString();
            string numTotal = numMessagesProcessed <= UNKNOWN ? NA : numMessagesProcessed.ToString();
// ReSharper restore SpecifyACultureInStringConversionExplicitly
            return string.Format(LOG_STRING, Environment.NewLine, new String('-', 34), 
                                             "Node Name:", name.PadLeft(24), 
                                             "# Msgs Good:", numGood.PadLeft(22),
                                             "# Msgs Broken:", numBroken.PadLeft(20), 
                                             "# Msgs Total:", numTotal.PadLeft(21), 
                                             "Constituent:",constituent.PadLeft(22));
        }
    }
}