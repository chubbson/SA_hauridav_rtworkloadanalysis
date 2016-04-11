using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core.Blocks;
using Dfn.Etl.Core.Tasks;
using Dfn.Etl.Crosscutting.ExceptionHandling;
using Dfn.Etl.Crosscutting.Logging;
using Dfn.Etl.Crosscutting.Logging.Statistics;

namespace Dfn.Etl.Core
{
    public class IdasDataflowNetwork2 : IIdasDataflowNetwork, IDisposable
    {
        private string m_Name;
        public virtual string Name { get { return m_Name; } protected set { m_Name = value; } }

        private ILogAgent m_LogAgent;
        public virtual ILogAgent LogAgent { get { return m_LogAgent; } protected set { m_LogAgent = value; } }

        private CancellationTokenSource m_Cts;
        public virtual CancellationTokenSource Cts { get { return m_Cts; } protected set { m_Cts = value; } }

        private bool m_LogStatistics;
        public virtual bool LogStatistics { get { return m_LogStatistics; } protected set { m_LogStatistics = value; } }

        private List<Task> m_PendingTasks;
        protected virtual List<Task> PendingTasks { get { return m_PendingTasks; } set { m_PendingTasks = value; } }

        private List<IBlock> m_CreatedBlocks; 

        protected bool m_Disposed;
        protected List<IStatisticsLogger> m_StatisticsLoggers;

        #region Ctor
        public IdasDataflowNetwork2(string name, ILogAgent logAgent, bool logStatistics = true)
        {
            m_Name = name;
            m_LogAgent = logAgent;
            m_Cts = new CancellationTokenSource();
            m_LogStatistics = logStatistics;
            m_PendingTasks = new List<Task>();
            m_StatisticsLoggers = new List<IStatisticsLogger>();
            m_CreatedBlocks = new List<IBlock>();
        }
        #endregion

        #region Configuration
        #endregion

        #region Operations

        public void Start(ISourceBlock sourceBlock)
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(null, "DataflowNetwork already disposed.");
            }

            try
            {
                //Begin processing the soruce
                StartProcessingSource(sourceBlock);

                // Block and wait for all the dataflow-network constituents to complete their work.
                var waitForTasks = PendingTasks.ToArray();
                if (waitForTasks.Length > 0)
                {
                    Task.WaitAll(waitForTasks.ToArray(), Cts.Token);
                }

                LogAgent.LogInfo(DataflowNetworkConstituent.Network, Name, "Network finished successfully.");

                OutputStatistics();

                LogAgent.Complete();
            }
            catch (AggregateException ex)
            {
                LogAgent.LogError(DataflowNetworkConstituent.Network, Name, ex.Flatten(), "The network encountered an unhandled exception.");
                throw new DataflowNetworkFailedException("The datanetwork " + Name + " failed. See the inner exception for more details", ex);
            }
            catch (OperationCanceledException)
            {
                var exceptions = PendingTasks.Where(task => task.IsFaulted).Select(task => task.Exception);
                foreach (var ex in exceptions)
                {
                    LogAgent.LogError(DataflowNetworkConstituent.Network, Name, ex.Flatten(), "The network encountered an unhandled exception.");
                }
            }
            catch (Exception ex)
            {
                LogAgent.LogError(DataflowNetworkConstituent.Network, Name, ex, "The network encountered an unhandled exception.");
                throw new DataflowNetworkFailedException("The datanetwork " + Name + " failed. See the inner exception for more details", ex);
            }
            finally
            {
                DisposePendingTasks();
                Dispose();
            }
        }

        private void DisposePendingTasks()
        {
            if(m_CreatedBlocks != null)
            {
                foreach (var block in m_CreatedBlocks)
                {
                    try
                    {
                        block.Dispose();
                    }
                    catch(Exception)
                    {
                        //Don't do anything
                    }
                }
            }
            PendingTasks.Clear();
        }

        protected virtual void StartProcessingSource(ISourceBlock sourceBlock)
        {
            var task = Task.Factory.StartNew(() => {
                sourceBlock.Start();
                sourceBlock.Complete();
            });

            //Add the task to be completed
            PendingTasks.Add(task);
        }


        public void CancelNetwork()
        {
            Cts.Cancel();
        }

        public virtual Task<bool> SendMessage<TMsg>(ITargetBlock<TMsg> target, TMsg msg)
        {
            return target.SendAsync(msg);
        }

        #endregion

        #region Block factory methods

        #region Buffer
        /// <summary>
        /// Creates a buffer that is meant to hold on to and forward stored items.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public virtual BufferBlock<TOutMsg> CreateBufferBlock<TOut, TOutMsg>(DataflowBlockOptions options = null)
            where TOutMsg : IDataflowMessage<TOut>
        {
            var block = new BufferBlock<TOutMsg>(GetBlockOptionsWithDefaults(options));
            return block;
        }
        #endregion

        #region Batch
        /// <summary>
        /// Creates a batch that groups items into the given batch size.
        /// After the batch size is reached the entire batch is forwarded to the next task in the dataflow-network.
        /// </summary>
        /// <typeparam name="TBatched"></typeparam>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public virtual BatchBlock<TOutMsg> CreateBatchBlock<TOut, TOutMsg>(int batchSize, GroupingDataflowBlockOptions options = null)
            where TOutMsg : IDataflowMessage<TOut>
        {
            options = GetBlockOptionsWithDefaults(options);
            var block = new BatchBlock<TOutMsg>(batchSize, options);
            return block;
        }

        #endregion

        #endregion

        #region Source
        public virtual SourceBlock<TOutput, TOutputMsg> SourceFor<TOutput, TOutputMsg>(IDataflowNetworkTask<TOutput, TOutputMsg, TOutput, TOutputMsg> source)
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            var task = DecorateTaskWithDefaults(source);

            var options = CreateDefaultExecutionBlockOptions();

            var block = new SourceBlock<TOutput, TOutputMsg>(this, options, task);

            m_CreatedBlocks.Add(block);

            return block;
        }
        #endregion

        #region Transform
        /// <summary>
        /// Creates a transformation that takes one input and produces one output for each.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public virtual TransformationBlock<TInput, TInputMsg, TOutput, TOutputMsg> TransformationFor<TInput, TInputMsg, TOutput, TOutputMsg>(
            IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> transformation
            )
            where TOutputMsg : IDataflowMessage<TOutput>
            where TInputMsg : IDataflowMessage<TInput>
        {
            var task = DecorateTaskWithDefaults(transformation);

            var options = CreateDefaultExecutionBlockOptions();

            var block = new TransformationBlock<TInput, TInputMsg, TOutput, TOutputMsg>(this, task, options);

            m_CreatedBlocks.Add(block);

            return block;
        }
        #endregion

        #region Target
        /// <summary>
        /// Creates a transformation that takes one input and and performs some task with it.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public TargetBlock<TInput, TInputMsg> TargetFor<TInput, TInputMsg>(ITargetTask<TInput, TInputMsg> target)
            where TInputMsg : IDataflowMessage<TInput>
        {
            var task = DecorateTaskWithDefaults(target);

            var options = CreateDefaultExecutionBlockOptions();

            var block = new TargetBlock<TInput, TInputMsg>(this, task, options);

            m_CreatedBlocks.Add(block);

            return block;
        }
        #endregion

        #region Finalizer
        /// <summary>
        /// Creates a transformation that takes one input and finalizes the flow at this.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public FinalizerTargetBlock<TInput, TInputMsg> FinalizerFor<TInput, TInputMsg>(ITargetTask<TInput, TInputMsg> target)
            where TInputMsg : IDataflowMessage<TInput>
        {
            var task = DecorateTaskWithDefaults(target);

            var options = CreateDefaultExecutionBlockOptions();

            var block = new FinalizerTargetBlock<TInput, TInputMsg>(this, task, options);

            m_CreatedBlocks.Add(block);

            return block;
        }
        #endregion

        #region Link
        public virtual void Link<TMsg>(ISourceBlock<TMsg> sourceBlock, ITargetBlock<TMsg> target, IdasDataflowLinkOptions linkOptions = null)
        {
            linkOptions = GetLinkOptionsWithDefaults(linkOptions);

            //Setup the link options
            GetLinkOptionsWithDefaults(linkOptions);

            //Now let the source block link
            sourceBlock.LinkTo(target, linkOptions);

            //Should we wait for the target?
            bool waitForTarget = false;
            if(linkOptions.WaitForTargetCompletion.HasValue)
                waitForTarget = linkOptions.WaitForTargetCompletion.Value;

            //Do not wait for finalizer blocks
            var finalizer = target as IFinalizerBlock;
            if (finalizer != null)
            {
                waitForTarget = false;
            }

            if (waitForTarget)
            {
                PendingTasks.Add(target.Completion);
            }
        }
        #endregion

        #region Block options
        public virtual ExecutionDataflowBlockOptions CreateDefaultExecutionBlockOptions(int boundedCapacity = -1)
        {
            var o = GetBlockOptionsWithDefaults(new ExecutionDataflowBlockOptions());
            if (boundedCapacity != -1)
            {
                o.BoundedCapacity = boundedCapacity;
            }
            return o;
        }

        public virtual GroupingDataflowBlockOptions CreateDefaultGroupingBlockOptions(int batchSize, int boundedCapacity = -1)
        {
            var o = GetGroupingBlockOptionsWithDefaults(new GroupingDataflowBlockOptions());
            var batchSizeMultiplier = boundedCapacity;
            if(boundedCapacity == -1)
                batchSizeMultiplier = 3;
            o.BoundedCapacity = batchSize * batchSizeMultiplier;
            return o;
        }

        public virtual DataflowBlockOptions CreateDefaultBlockOptions(int boundedCapacity = -1)
        {
            var o = GetBlockOptionsWithDefaults(new DataflowBlockOptions());
            if(boundedCapacity != -1)
            {
                o.BoundedCapacity = boundedCapacity;
            }
            return o;
        }

        protected virtual TOptions GetBlockOptionsWithDefaults<TOptions>(TOptions blockOptions)
            where TOptions : DataflowBlockOptions, new()
        {
            if (blockOptions == null)
                blockOptions = new TOptions();

            //Setup the cancellation token
            if (m_Cts != null)
            {
                blockOptions.CancellationToken = m_Cts.Token;
            }

            /*
            blockOptions.TaskScheduler = Scheduler;
            blockOptions.MaxMessagesPerTask = TaskMaxMessages;
            blockOptions.BoundedCapacity = DefaultTaskBoundedCapacity;
            */

            return blockOptions;
        }

        protected virtual TOptions GetExecutionBlockOptionsWithDefaults<TOptions>(TOptions blockOptions)
            where TOptions : ExecutionDataflowBlockOptions, new()
        {
            blockOptions = GetBlockOptionsWithDefaults(blockOptions);
            return blockOptions;
        }

        protected virtual TOptions GetGroupingBlockOptionsWithDefaults<TOptions>(TOptions blockOptions)
            where TOptions : GroupingDataflowBlockOptions, new()
        {
            blockOptions = GetBlockOptionsWithDefaults(blockOptions);
            return blockOptions;
        }

        protected virtual TOptions GetLinkOptionsWithDefaults<TOptions>(TOptions linkOptions)
            where TOptions : IdasDataflowLinkOptions, new()
        {
            if (linkOptions == null)
                linkOptions = new TOptions();

            if (!linkOptions.WaitForTargetCompletion.HasValue)
                linkOptions.WaitForTargetCompletion = true;

            return linkOptions;
        }

        protected virtual IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> DecorateTaskWithDefaults<TInput, TInputMsg, TOutput, TOutputMsg>(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> task)
            where TOutputMsg : IDataflowMessage<TOutput>
            where TInputMsg : IDataflowMessage<TInput>
        {
            task = task
                .WithExceptionHandler(this, m_LogAgent)
                .SkipEmpty(this, m_LogAgent)
                .WithLogging(this, m_LogAgent);

            if (m_LogStatistics)
            {
                var withStatisticsTask = task.WithStatistics(this, m_LogAgent);
                m_StatisticsLoggers.Add(withStatisticsTask);
                task = withStatisticsTask;
            }

            //Finallly make sure not to pass the broken messages to the next task...
            task = task.SkipBroken(this, m_LogAgent);

            return task;
        }

        #endregion

        #region Internal helper methods

        /// <summary>
        /// Tells all registered statistics loggers to dump their stats to the logfile.
        /// </summary>
        private void OutputStatistics()
        {
            foreach (var statisticsLogger in m_StatisticsLoggers)
            {
                statisticsLogger.LogStatistics();
            }
        }

        #endregion

        #region Disposable
        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }

            //Dispose the cancellation token source
            Cts.Dispose();

            m_Disposed = true;
        }
        #endregion
    }
}
