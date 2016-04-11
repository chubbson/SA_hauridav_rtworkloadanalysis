using System;
using Common.Logging;
using Dfn.Etl.Core;

namespace Dfn.Etl.Dataflow.Actions
{
    public sealed class EnrichFilesWithSeqNr : ITransformation<string, Tuple<string, int>>
    {
        private readonly int m_seqNrSeed;
        private int m_seqNr;
        private readonly ILog m_Log;

        public EnrichFilesWithSeqNr(ILog logger, int seqNrSeed = 0)
        {
            m_seqNrSeed = seqNrSeed;
            m_seqNr = m_seqNrSeed;
            m_Log = logger;
        }

        public string Title
        {
            get { return string.Format("Enrich Files with SequenceNr"); }
        }

        public Tuple<string, int> Transform(string item)
        {
            m_Log.Info(String.Format("Enrich '{0}' with sequence '{1}'", item, m_seqNr));
            var result = new Tuple<string, int>(item, m_seqNr);

            m_seqNr++;

            return result;
        }
    }
}
