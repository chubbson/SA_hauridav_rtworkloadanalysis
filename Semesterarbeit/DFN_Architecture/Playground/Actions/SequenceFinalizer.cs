using System;
using Common.Logging;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    //public sealed class SequenceEnricher : ITransformation<Tuple<UInt64, UInt64, string>, Tuple<UInt64, UInt64, string>>
    //{
    //    string m_filename;
    //}

    public class SequenceFinalizer : ITarget<Tuple<UInt64, UInt64, string>>
    {
        private ILog m_Log;
        private int m_i;

        public SequenceFinalizer(ILog log)
        {
            m_Log = log;
            m_i = 0;
        }


        public string Title
        {
            get { return "Target finalization, endstep of the dataflow pipeline"; }
        }

        public void Push(Tuple<UInt64, UInt64, string> item)
        {

            m_i++;



            if (item.Item1 % 1000 == 0)
                m_Log.Info(++m_i);


            // Cleanup stuff
            if (item != null)
            {
                item = null;
            }

            if (m_i % 10000 == 0)
            {
                CollectGarbageForced();
            }
            else
            {
                CollectGarbageOptimized();
            }


            //CollectGarbageOptimized();
        }

        protected virtual void CollectGarbageOptimized()
        {
            GC.Collect(2, GCCollectionMode.Optimized);
        }

        protected virtual void CollectGarbageForced()
        {
            GC.Collect(2, GCCollectionMode.Forced);
        }
    }

    public class SequenceFinalizer<TIn> : ITarget<TIn>
    {
        public string Title
        {
            get { return "Target finalization, endstep of the dataflow pipeline"; }
        }

        public void Push(TIn item)
        {
            CollectGarbage();
        }

        protected virtual void CollectGarbage()
        {
            GC.Collect(2, GCCollectionMode.Forced);
        }
    }
}
