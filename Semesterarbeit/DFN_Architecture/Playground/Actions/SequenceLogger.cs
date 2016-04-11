using System;
using Common.Logging;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    public sealed class SequenceLogger : ITransformation<Tuple<UInt64, UInt64, string>, Tuple<UInt64, UInt64, string>>
    {
        private ILog m_Log;

        public SequenceLogger(ILog log)
        {
            m_Log = log;
        }

        public string Title
        {
            get { return "Sequence Logger Task"; }
        }

        public Tuple<UInt64, UInt64, string> Transform(Tuple<UInt64, UInt64, string> input)
        {
            m_Log.Debug(input.Item3);

            return input;
        }
    }
}
