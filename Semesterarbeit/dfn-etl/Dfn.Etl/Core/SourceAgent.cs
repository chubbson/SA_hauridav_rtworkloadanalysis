using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a default implementation of an independent source.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public sealed class SourceAgent<TOut> : ISourceAgent<TOut>
    {
        /// <summary>
        /// The buffer that is filled with the produced items.
        /// </summary>
        private readonly BufferBlock<TOut> m_OutgoingMessages;

        /// <summary>
        /// The actual producer.
        /// </summary>
        private readonly Func<IEnumerable<TOut>> m_SourceFunc;

        /// <summary>
        /// The token that signals to this agent when the producing must be halted.
        /// </summary>
        private readonly CancellationToken m_CancellationToken;
        private Predicate<TOut> m_Predicate;

        public SourceAgent(Func<IEnumerable<TOut>> sourceFunc, CancellationToken cancellationToken, DataflowBlockOptions options)
        {
            m_OutgoingMessages = new BufferBlock<TOut>(options);
            m_SourceFunc = sourceFunc;
            m_CancellationToken = cancellationToken;
        }

        public int Outputcount
        {
            get { return m_OutgoingMessages.Count; }
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(StartCore);
        }
        
        private void StartCore()
        {
            try
            {
                var sourceEnumerator = m_SourceFunc();

                foreach (var item in sourceEnumerator.Where(item => m_Predicate(item)))
                {
                    m_OutgoingMessages.SendAsync(item, m_CancellationToken).Wait();
                }
            }
            finally
            {
                // Mark as complete to propagate the completion status to the successor tasks.
                m_OutgoingMessages.Complete();
            }
        }

        public void LinkTo(ITargetBlock<TOut> target, DataflowLinkOptions linkOptions, Predicate<TOut> predicate)
        {
            // We do not pass the BufferBlock the predicate because we implement the predicate differently.
            // The difference is that we throw away non-matching items whereas the BufferBlock does not.
            m_OutgoingMessages.LinkTo(target, linkOptions);
            m_Predicate = predicate;
        }
    }

    public sealed class SourceAgent2<TOut> : ISourceAgent<TOut>
    {
        private readonly ISource<TOut> m_Source;
        // Responsible for flattening the source data
        private readonly TransformManyBlock<ISource<TOut>, TOut> m_Flattener =
            new TransformManyBlock<ISource<TOut>, TOut>(src => src.Pull());

        public SourceAgent2(ISource<TOut> source)
        {
            m_Source = source;
        }

        public Task StartAsync()
        {
            return m_Flattener.SendAsync(m_Source);
        }

        public void LinkTo(ITargetBlock<TOut> target, DataflowLinkOptions linkOptions, Predicate<TOut> predicate)
        {
            m_Flattener.LinkTo(target, linkOptions, predicate);
        }
    }
}
