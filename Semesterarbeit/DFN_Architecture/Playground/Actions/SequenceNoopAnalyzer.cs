using System;
using Common.Logging;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    public sealed class SequenceNoopAnalyzer : ITransformation<Tuple<UInt64, UInt64, string>, Tuple<UInt64, UInt64, string>>
    {
        private ILog m_Log;
        private int m_i;

        public SequenceNoopAnalyzer(ILog log)
        {
            m_Log = log;
            m_i = 0;
        }

        public string Title
        {
            get { return "Sequence Noop Analyzer Task"; }
        }

        public Tuple<UInt64, UInt64, string> Transform(Tuple<UInt64, UInt64, string> input)
        {
            if (input.Item1 % 1000 == 0)
                m_Log.Info(++m_i);

            return input;
        }
    }
}
