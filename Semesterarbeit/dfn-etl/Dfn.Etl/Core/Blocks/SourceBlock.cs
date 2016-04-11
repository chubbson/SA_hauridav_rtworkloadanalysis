using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Core.Blocks
{
    public class SourceBlock<TBlock, TOut, TMsg> : BaseInputOutputBlockWithTarget<TBlock, TOut, TMsg, TOut, TMsg>, ISourceBlock
        where TMsg : IDataflowMessage<TOut>
    {
        private object m_SourceSyncObj = new object();
        private bool m_HasProcessedSources;

        private IDataflowNetworkTask<TOut, TMsg, TOut, TMsg> m_Source;
        public virtual IDataflowNetworkTask<TOut, TMsg, TOut, TMsg> Source
        {
            get
            {
                return m_Source;
            }
            protected set
            {
                m_Source = value;
            }
        }

        #region Ctor

        public SourceBlock(IIdasDataflowNetwork network, ExecutionDataflowBlockOptions options, IDataflowNetworkTask<TOut, TMsg, TOut, TMsg> source)
            : base(network, null, options)
        {
            m_Source = source;
        }

        public SourceBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TOut, TMsg, TOut, TMsg> source)
            : base(network, null, null)
        {
            m_Source = source;
        }
        #endregion

        #region Initialize

        protected override IEnumerable<TMsg> ProcessInputMessages(IEnumerable<TMsg> msgs)
        {
            return msgs;
        }

        #endregion

        public virtual void Start()
        {
            ProcessSources();            
        }

        private void ProcessSources()
        {
            if (m_HasProcessedSources)
                return;

            lock (m_SourceSyncObj)
            {
                if(m_HasProcessedSources)
                    return;

                m_HasProcessedSources = true;
            }

            if(m_Source != null)
            {
                var pulledItems = m_Source.Process(null);
                foreach (var item in pulledItems)
                {
                    SendOutput(item);
                }
            }
        }

        protected override void OnSourceCompletion()
        {
            base.OnSourceCompletion();
            
            if (m_Source == null)
                return;

            try
            {
                m_Source.EndProcessingTask();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to end processing source task.", e);
            }
        }

        protected override void OnTargetCompletion()
        {
            base.OnTargetCompletion();

            ProcessSources();
        }

        public override string ToString()
        {
            if(m_Source == null)
                return base.ToString();

            return m_Source.ToString();
        }

        public override void Dispose()
        {
            base.Dispose();

            if(Source != null)
            {
                Source.Dispose();
            }
        }
    }

    public class SourceBlock<TOut, TMsg> : SourceBlock<SourceBlock<TOut, TMsg>, TOut, TMsg>
        where TMsg : IDataflowMessage<TOut>
    {
        public SourceBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TOut, TMsg, TOut, TMsg> source)
            : base(network, source)
        {
        }

        public SourceBlock(IIdasDataflowNetwork network, ExecutionDataflowBlockOptions options, IDataflowNetworkTask<TOut, TMsg, TOut, TMsg> source)
            : base(network, options, source)
        {
        }
    }

    public class SourceBlock<TOut> : SourceBlock<SourceBlock<TOut>, TOut, IDataflowMessage<TOut>>
    {
        public SourceBlock(IIdasDataflowNetwork network, ExecutionDataflowBlockOptions options, IDataflowNetworkTask<TOut, IDataflowMessage<TOut>, TOut, IDataflowMessage<TOut>> source)
            : base(network, options, source)
        {
        }

        public SourceBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TOut, IDataflowMessage<TOut>, TOut, IDataflowMessage<TOut>> source)
            : base(network, source)
        {
        }
    }
}
