using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Dfn.Etl.Core.Blocks
{
    public abstract class BaseInputOutputBlock<TBlock, TInput, TInputMsg, TOutput, TOutputMsg> : 
        IPropagatorBlock<TInputMsg, TOutputMsg>,
        IReceivableSourceBlock<TOutputMsg>,
        ISourceBlock<TOutputMsg>,
        IBlock
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        private object m_InitSyncObj = new object();
        private bool m_IsInitialized;

        private IIdasDataflowNetwork m_Network;
        public virtual IIdasDataflowNetwork Network
        {
            get
            {
                return m_Network;
            }
            protected set
            {
                m_Network = value;
            }
        }

        public int InputBatchSize { get; protected set; }

        private BufferBlock<TOutputMsg> m_Source;
        protected virtual BufferBlock<TOutputMsg> Source
        {
            get
            {
                if (m_Source == null)
                {
                    VerifyInitialized();
                }

                return m_Source;
            }
            set
            {
                m_Source = value;
            }
        }

        private ITargetBlock<TInputMsg> m_Target;
        protected virtual ITargetBlock<TInputMsg> Target
        {
            get
            {
                if (m_Target == null)
                {
                    VerifyInitialized();
                }

                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }

        private DataflowBlockOptions m_SourceOptions;
        public virtual DataflowBlockOptions SourceOptions
        {
            get { return m_SourceOptions; }
            protected set { m_SourceOptions = value; }
        }

        private ExecutionDataflowBlockOptions m_TargetOptions;
        public virtual ExecutionDataflowBlockOptions TargetOptions
        {
            get { return m_TargetOptions; }
            protected set { m_TargetOptions = value; }
        }

        private object m_OutputMsgsSyncObj = new object();
        private Queue<QueueEntry<TOutput, TOutputMsg>> m_OutputMsgsQueue;

        #region Ctor
        protected BaseInputOutputBlock(IIdasDataflowNetwork network, DataflowBlockOptions sourceOptions, ExecutionDataflowBlockOptions targetOptions)
        {
            m_Network = network;
            m_SourceOptions = sourceOptions;
            m_TargetOptions = targetOptions;
            m_OutputMsgsQueue = new Queue<QueueEntry<TOutput, TOutputMsg>>();
        }
        #endregion

        #region Initialize
        protected virtual void VerifyInitialized()
        {
            var doInit = false;
            lock (m_InitSyncObj)
            {
                doInit = !m_IsInitialized;
            }

            if (doInit)
            {
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            lock (m_InitSyncObj)
            {
                if (m_IsInitialized)
                    return;
                m_IsInitialized = true;

                InitializeSource();
                InitializeTarget();
            }
        }

        private void InitializeSource()
        {
            CreateSource();

            Source.Completion.ContinueWith(task => {
                OnSourceCompletion();
            });
        }

        private void InitializeTarget()
        {
            CreateTarget();

            if(InputBatchSize > 1)
            {
                //Setup batched target
                var batchBlock = Network.CreateBatchBlock<TInput, TInputMsg>(InputBatchSize);
                
                var batchedMessagesToSingleMsg = new ActionBlock<TInputMsg[]>(msgs => {
                    if(msgs == null)
                        return;

                    var msgsEntry = new QueueEntry<TOutput, TOutputMsg>();
                    lock (msgsEntry.SyncObj)
                    {
                        AddOutputMessages(msgsEntry);
                        HandleMessages(msgsEntry, msgs);
                    }

                    ProcessOutputQueueEntries();
                });

                batchBlock.Completion.ContinueWith(task => {
                    //Now complete the single transformation
                    batchedMessagesToSingleMsg.Complete();
                });

                batchedMessagesToSingleMsg.Completion.ContinueWith(task => {
                    //Now complete the output source
                    Source.Complete();
                });

                //Link the batch block - to the output generating action
                Network.Link(batchBlock, batchedMessagesToSingleMsg);

                //The new target is the batch block
                Target = batchBlock;
            }
            else
            {
                Target.Completion.ContinueWith(task => {
                    OnTargetCompletion();

                    //Now complete the output source
                    Source.Complete();
                });
            }
        }

        protected virtual void OnTargetCompletion()
        {
            //Make sure to finish whatever has been generated by the batcher
            ProcessOutputQueueEntries(true);
        }

        protected virtual void OnSourceCompletion()
        {
            //Make sure to finish whatever has been generated by the batcher
            ProcessOutputQueueEntries(true);
        }

        protected abstract IEnumerable<TOutputMsg> ProcessInputMessages(IEnumerable<TInputMsg> msgs);

        protected virtual void CreateSource()
        {
            var options = SourceOptions;
            if(options == null)
            {
                options = CreateDefaultSourceOptions();
            }

            m_Source = Network.CreateBufferBlock<TOutput, TOutputMsg>(options);
        }

        protected virtual void CreateTarget()
        {
            var options = TargetOptions;
            if (options == null)
                options = CreateDefaultTargetOptions();

            m_Target = new ActionBlock<TInputMsg>((TInputMsg inMsg) => {
                if(inMsg == null)
                    return;

                var msgsEntry = new QueueEntry<TOutput, TOutputMsg>();
                lock (msgsEntry.SyncObj)
                {
                    AddOutputMessages(msgsEntry);
                    HandleMessages(msgsEntry, inMsg);
                }

                ProcessOutputQueueEntries();
            }, options);
        }

        protected virtual void HandleMessages(QueueEntry<TOutput, TOutputMsg> msgsEntry, params TInputMsg[] msgs)
        {
            //Make the transformation
            var outputMsgs = ProcessInputMessages(msgs);

            //Add all the output messages
            msgsEntry.Messages.AddRange(outputMsgs);
        }
        #endregion

        #region Configuration
        public virtual TBlock BoundedCapacity(int capacity)
        {
            if(TargetOptions == null)
            {
                TargetOptions = CreateDefaultTargetOptions();
            }

            if(TargetOptions != null)
            {
                //Set the batch size
                TargetOptions.BoundedCapacity = capacity;
            }

            return (TBlock)(object)this;
        }

        public virtual TBlock BatchedInput(int batchSize)
        {
            InputBatchSize = batchSize;
            return (TBlock)(object)this;
        }

        protected virtual DataflowBlockOptions CreateDefaultSourceOptions()
        {
            return Network.CreateDefaultBlockOptions();
        }

        protected virtual ExecutionDataflowBlockOptions CreateDefaultTargetOptions()
        {
            return Network.CreateDefaultExecutionBlockOptions();
        }

        #endregion

        #region Output message processing
        protected virtual void AddOutputMessages(QueueEntry<TOutput, TOutputMsg> msgsEntry)
        {
            m_OutputMsgsQueue.Enqueue(msgsEntry);
        }

        protected virtual void ProcessOutputQueueEntries(bool wait = false)
        {
            if (m_OutputMsgsQueue == null)
                return;

            while(true)
            {
                QueueEntry<TOutput, TOutputMsg> msgsEntry = null;
                lock(m_OutputMsgsSyncObj)
                {
                    if(m_OutputMsgsQueue.Count <= 0)
                        return;

                    msgsEntry = m_OutputMsgsQueue.Dequeue();
                }
                
                lock(msgsEntry.SyncObj)
                {
                    if(msgsEntry.IsProcessed)
                        continue;

                    msgsEntry.IsProcessed = true;

                    foreach(var msg in msgsEntry.Messages)
                    {
                        SendOutput(msg, wait);
                    }
                }
            }
        }

        protected virtual void SendOutput(TOutputMsg msg, bool wait = false)
        {
            //Let the newtork decide how to send message - for example synchronously or asynchronously
            var sent = Network.SendMessage(Source, msg);
            if (wait)
                sent.Wait();
        }
        #endregion

        #region Block implementation
        public virtual void Complete()
        {
            //First finish any pending queue entries...
            ProcessOutputQueueEntries();

            //Console.WriteLine("Complete for: " + this);

            //Complete the target
            Target.Complete();
        }

        public virtual Task Completion
        {
            get
            {
                if (Source == null)
                    return null;

                return Source.Completion;
            }
        }

        public virtual void Fault(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            if (Target == null)
                return;

            ((IDataflowBlock)Target).Fault(exception);
        }

        IDisposable ISourceBlock<TOutputMsg>.LinkTo(ITargetBlock<TOutputMsg> target, DataflowLinkOptions linkOptions)
        {
            return Link(target, linkOptions);
        }

        public virtual TBlock LinkTo(ITargetBlock<TOutputMsg> target, IdasDataflowLinkOptions linkOptions = null)
        {
            Network.Link(this, target, linkOptions);

            return (TBlock)(object)this;
        }

        protected virtual IDisposable Link(ITargetBlock<TOutputMsg> target, DataflowLinkOptions linkOptions)
        {
            Initialize();

            if (Source == null)
                throw new Exception("ContentSource has not been initialized!");

            //Always propagate the completion
            linkOptions.PropagateCompletion = true;

            var linked = Source.LinkTo(target, linkOptions);

            return linked;
        }

        public virtual DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, TInputMsg messageValue, ISourceBlock<TInputMsg> source, bool consumeToAccept)
        {
            if (Target == null)
                return DataflowMessageStatus.NotAvailable;

            return ((ITargetBlock<TInputMsg>)Target).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }


        public virtual bool TryReceive(Predicate<TOutputMsg> filter, out TOutputMsg item)
        {
            item = default(TOutputMsg);

            var receivableSourceBlock = Source as IReceivableSourceBlock<TOutputMsg>;
            if (receivableSourceBlock != null)
                return receivableSourceBlock.TryReceive(filter, out item);

            return false;
        }

        public virtual bool TryReceiveAll(out IList<TOutputMsg> items)
        {
            items = null;

            var receivableSourceBlock = Source as IReceivableSourceBlock<TOutputMsg>;
            if (receivableSourceBlock != null)
                return receivableSourceBlock.TryReceiveAll(out items);

            return false;
        }

        public virtual TOutputMsg ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutputMsg> target, out bool messageConsumed)
        {
            messageConsumed = false;
            if (Source == null)
                return default(TOutputMsg);

            return ((ISourceBlock<TOutputMsg>)Source).ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public virtual bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutputMsg> target)
        {
            if (Source == null)
                return false;

            return ((ISourceBlock<TOutputMsg>)Source).ReserveMessage(messageHeader, target);
        }

        public virtual void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutputMsg> target)
        {
            if (Source == null)
                return;

            ((ISourceBlock<TOutputMsg>)Source).ReleaseReservation(messageHeader, target);
        }
        #endregion

        #region Helper classes
        protected class QueueEntry<TOut, TMsg>
            where TMsg: IDataflowMessage<TOut>
        {
            public object SyncObj { get; private set; }
            public bool IsLocked { get; protected set; }
            public List<TMsg> Messages { get; set; }
            public bool IsProcessed { get; set; }

            public QueueEntry()
            {
                SyncObj = new object();
                Messages = new List<TMsg>();
            }

            public bool Lock()
            {
                bool aquiredLock = false;
                lock(SyncObj)
                {
                    aquiredLock = !IsLocked;
                    IsLocked = true;
                }
                return aquiredLock;
            }
        }
        #endregion

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public virtual void Dispose()
        {
        }
    }

    public abstract class BaseInputOutputBlockWithTarget<TBlock, TIn, TInMsg, TOut, TOutMsg> : BaseInputOutputBlock<TBlock, TIn, TInMsg, TOut, TOutMsg>
        where TInMsg : IDataflowMessage<TIn>
        where TOutMsg : IDataflowMessage<TOut>
    {
        protected BaseInputOutputBlockWithTarget(IIdasDataflowNetwork network, DataflowBlockOptions sourceOptions, ExecutionDataflowBlockOptions targetOptions)
            : base(network, sourceOptions, targetOptions)
        {
        }
    }
}
