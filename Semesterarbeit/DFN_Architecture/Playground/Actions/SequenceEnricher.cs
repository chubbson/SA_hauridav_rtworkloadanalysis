using System;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    public sealed class SequenceEnricher : ITransformation<Tuple<UInt64, UInt64>, Tuple<UInt64, UInt64, string>>
    {

        public SequenceEnricher()
        {
            
        }

        public string Title
        {
            get { return "Sequence Enricher Task"; }
        }

        public Tuple<UInt64, UInt64, string> Transform(Tuple<UInt64, UInt64> input)
        {
            var res = new Tuple<UInt64, UInt64, string>(input.Item1, input.Item2, string.Format("Sequence: {0}, Value: {1}", input.Item1, input.Item2));
            return res;
        }
    }
}
