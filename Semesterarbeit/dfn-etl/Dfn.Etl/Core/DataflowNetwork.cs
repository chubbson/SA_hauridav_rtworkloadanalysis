using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Crosscutting.ExceptionHandling;
using Dfn.Etl.Crosscutting.Logging;
using Dfn.Etl.Crosscutting.Logging.Statistics;
using Dfn.Etl.Crosscutting.WorkloadStatistics;
using ZeroMQ;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a default implementation for a dataflow-network.
    /// It knows how to create network constituents as well as how to link them together using sane default options.
    /// Use this object to create and link together your dataflow-network.
    /// </summary>
    public sealed class DataflowNetwork : IDataflowNetwork, ICancelNetwork, IDisposable
    {
        private readonly Guid m_DfnGuid;
        private readonly string m_Name;
        private readonly CancellationTokenSource m_Cts;
        private readonly ILogAgent m_LogAgent;
        private readonly bool m_LogStatistics;
        private ISourceAgent m_SourceAgent;
        private readonly ZContext m_ctx;

        /// <summary>
        /// A list of tasks that represent the work to be done by each created dataflow block.
        /// We are saving them to be able to Wait() on the completion of them all.
        /// </summary>
        private readonly List<Task> m_ConstituentTasks;
        private readonly List<Action> m_NullLinks = new List<Action>();
        private readonly List<IStatisticsLogger> m_StatisticsLoggers = new List<IStatisticsLogger>();

        private DataflowNetwork() {}
        public DataflowNetwork(string name, ILogAgent logAgent, bool logStatistics = true)
        {
            m_DfnGuid = Guid.NewGuid();
            m_Name = name;
            m_Cts = new CancellationTokenSource();
            m_LogAgent = logAgent;
            m_LogStatistics = logStatistics;
            m_ConstituentTasks = new List<Task>();
            m_ctx = new ZContext();
        }

        /// <summary>
        /// Activates the flow of messages inside the dataflow-network. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <exception cref="DataflowNetworkFailedException">Thrown when the dataflow-network has encountered an exception during its lifetime.</exception>
        public void Start<T>(ISourceAgent<T> source)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(null, "DataflowNetwork already disposed.");
            }

            if (m_SourceAgent == null || m_SourceAgent != source)
            {
                throw new ArgumentException("SourceAgent not linked with this network.", "source");
            }

            // Attach sinks.
            foreach (var nullLink in m_NullLinks)
            {
                nullLink();
            }

            try
            {
                // Tell the producer to start producing items.
                var mainTask = source.StartAsync();
                m_ConstituentTasks.Add(mainTask);

                // Block and wait for all the dataflow-network constituents to complete their work.
                Task.WaitAll(m_ConstituentTasks.ToArray(), m_Cts.Token);
                m_LogAgent.LogInfo(DataflowNetworkConstituent.Network, m_Name, "Network finished successfully.");
                DumpStats();
                m_LogAgent.Complete();
            }
            catch (AggregateException ex)
            {
                m_LogAgent.LogError(DataflowNetworkConstituent.Network, m_Name, ex.Flatten(), "The network encountered an unhandled exception.");
                throw new DataflowNetworkFailedException("The datanetwork " + m_Name + " failed. See the inner exception for more details", ex);
            }
            catch (OperationCanceledException)
            {
                var exceptions = m_ConstituentTasks.Where(task => task.IsFaulted).Select(task => task.Exception);
                foreach (var ex in exceptions)
                {
                    m_LogAgent.LogError(DataflowNetworkConstituent.Network, m_Name, ex.Flatten(),
                                        "The network encountered an unhandled exception.");
                }
            }
            catch (Exception ex)
            {
                m_LogAgent.LogError(DataflowNetworkConstituent.Network, m_Name, ex, "The network encountered an unhandled exception.");
                throw new DataflowNetworkFailedException("The datanetwork " + m_Name + " failed. See the inner exception for more details", ex);
            }
            finally
            {
                m_ConstituentTasks.Clear();
                m_Cts.Cancel();
                GC.Collect();
                Dispose();
            }
        }

        public void CancelNetwork()
        {
            m_Cts.Cancel();
        }

        /// <summary>
        /// Creates a source that is capable of producing items.
        /// </summary>
        /// <typeparam name="TOut">The type of the item that is produced by the source.</typeparam>
        /// <param name="source">The producer.</param>
        /// <param name="boundedCapacity">The amount of items the producer stores in its output queue.</param>
        /// <returns></returns>
        public ISourceAgent<IDataflowMessage<TOut>> CreateSource<TOut>(ISource<TOut> source, int boundedCapacity = -1)
        {
            /*
            var agent = new SourceAgent<IDataflowMessage<TOut>>(
                source
                    .WithExceptionHandler(m_LogAgent, this)
                    .WithLogging(m_LogAgent)
                    .AsFunction(), m_Cts.Token, CreateDefaultBlockOptions(boundedCapacity));
            */
            SourceAgent<IDataflowMessage<TOut>> agent = null;

            var srcFunc = source
                            .WithExceptionHandler(m_LogAgent, this)
                            .WithLogging(m_LogAgent)
                            .WithWorkloadStatistiscs(boundedCapacity, ()=>agent.Outputcount, m_ctx, this.m_DfnGuid, m_Cts.Token) ;

            if (m_LogStatistics)
            {
                srcFunc = srcFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)srcFunc);
            }

            agent =
                new SourceAgent<IDataflowMessage<TOut>>(srcFunc.AsFunction(), m_Cts.Token,
                    CreateDefaultBlockOptions(boundedCapacity));
            
            return agent;
        }
        /*
             var transFunc = transformation
                .WithExceptionHandler(m_LogAgent, this)
                .WithLogging(m_LogAgent)
                ;

            if (m_LogStatistics)
            {
                transFunc = transFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)transFunc);
            }
            
            var block = CustomBlocks.CreateThrottledTransformMany(
                transFunc
                    .AsFunction(), CreateDefaultExecutionOptions(boundedCapacity)
                );

            return block;
        */


        /// <summary>
        /// Creates a buffer that is meant to hold on to and forward stored items.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public BufferBlock<IDataflowMessage<TOut>> CreateBuffer<TOut>(int boundedCapacity = -1)
        {
            var block = new BufferBlock<IDataflowMessage<TOut>>(CreateDefaultBlockOptions(boundedCapacity));
            return block;
        }

        /// <summary>
        /// Creates a transformation that takes one input and produces one output for each.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public TransformBlock<IDataflowMessage<TIn>, IDataflowMessage<TOut>> CreateTransform<TIn, TOut>(ITransformation<TIn, TOut> transformation, int boundedCapacity = -1)
        {
            TransformBlock<IDataflowMessage<TIn>, IDataflowMessage<TOut>> block = null; 

            ITransformationFunctor<TIn, TOut> transFunc = transformation
                .WithExceptionHandler(m_LogAgent, this)
                .WithLogging(m_LogAgent)
                .WithWorkloadStatistics(boundedCapacity, () => block.InputCount, () => block.OutputCount, m_ctx, this.m_DfnGuid, m_Cts.Token);// x() f(), functionPointer())// MsgInFunc(), MsgInFunc())// m_MsgInFunc, m_MsgOutFunc)

            if (m_LogStatistics)
            {
                transFunc = transFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)transFunc);
            }
            
            block = new TransformBlock<IDataflowMessage<TIn>, IDataflowMessage<TOut>>(
               transFunc
                .AsFunction(), CreateDefaultExecutionOptions(boundedCapacity));
             
            return block;
        }

        /// <summary>
        /// Creates a transformation that takes one batched input and produces one output for.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public TransformBlock<IDataflowMessage<TIn>[], IDataflowMessage<TOut>> CreateTransformBatched<TIn, TOut>(ITransformationBatched<TIn, TOut> transformation, int boundedCapacity = -1)
        {
            TransformBlock<IDataflowMessage<TIn>[], IDataflowMessage<TOut>> block = null;

            var transFunc = transformation
                .WithExceptionHandler(m_LogAgent, this)
                .WithLogging(m_LogAgent)
                .WithWorkloadStatistics(boundedCapacity, () => block.InputCount, () => block.OutputCount, m_ctx, this.m_DfnGuid, m_Cts.Token);

            if (m_LogStatistics)
            {
                transFunc = transFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)transFunc);
            }

            block = new TransformBlock<IDataflowMessage<TIn>[], IDataflowMessage<TOut>>(
               transFunc
                .AsFunction(), CreateDefaultExecutionOptions(boundedCapacity));
     
            return block;
        }

        public IPropagatorBlock<IDataflowMessage<TIn>, IDataflowMessage<TOut>> CreateDelayingTransformMany<TIn, TOut>(ITransformMany<TIn, TOut> transformation, int boundedCapacity = -1)
        {
            var transFunc = transformation
                .WithExceptionHandler(m_LogAgent, this)
                .WithLogging(m_LogAgent)
                //.WithWorkloadStatistics(boundedCapacity, ()=> )
                ;

            if (m_LogStatistics)
            {
                transFunc = transFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)transFunc);
            }
            
            var block = CustomBlocks.CreateThrottledTransformMany(
                transFunc
                    .AsFunction(), CreateDefaultExecutionOptions(boundedCapacity)
                );

            return block;
        }

        /// <summary>
        /// Creates a target that takes one input.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="target"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public ActionBlock<IDataflowMessage<TIn>> CreateTarget<TIn>(ITarget<TIn> target, int boundedCapacity = -1)
        {
            ActionBlock<IDataflowMessage<TIn>> block = null;
            var targetFunc = target
                .WithExceptionHandler(m_LogAgent, this)
                .WithLogging(m_LogAgent)
                .WithWorkloadStatistics(boundedCapacity, () => block.InputCount, m_ctx, this.m_DfnGuid, m_Cts.Token);
                
            if (m_LogStatistics)
            {
                targetFunc = targetFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)targetFunc);
            }

            block = new ActionBlock<IDataflowMessage<TIn>>(
                targetFunc
                    .AsFunction(), CreateDefaultExecutionOptions(boundedCapacity));
            RecordTask(block);
            return block;
        }

        /// <summary>
        /// Creates a target that takes a group of items.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="target"></param>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        public ActionBlock<IDataflowMessage<TIn>[]> CreateTargetBatched<TIn>(ITargetBatched<TIn> target, int boundedCapacity = -1)
        {
            ActionBlock<IDataflowMessage<TIn>[]> block = null;
            var targetFunc = target
                .WithExceptionHandler(m_LogAgent, this)
                .WithLogging(m_LogAgent)
                .WithWorkloadStatistics(boundedCapacity, () => block.InputCount, m_ctx, this.m_DfnGuid, m_Cts.Token);

            if (m_LogStatistics)
            {
                targetFunc = targetFunc.WithStatistics(m_LogAgent);
                m_StatisticsLoggers.Add((IStatisticsLogger)targetFunc);
            }

            block = new ActionBlock<IDataflowMessage<TIn>[]>(
                targetFunc
                    .AsFunction(), CreateDefaultExecutionOptions(boundedCapacity));
            RecordTask(block);
            return block;
        }

        /// <summary>
        /// Creates a batch that groups items into the given batch size.
        /// After the batch size is reached the entire batch is forwarded to the next task in the dataflow-network.
        /// </summary>
        /// <typeparam name="TBatched"></typeparam>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public BatchBlock<IDataflowMessage<TBatched>> CreateBatch<TBatched>(int batchSize, int outputBoundedCapacity = 1)
        {
            var block = new BatchBlock<IDataflowMessage<TBatched>>(batchSize, new GroupingDataflowBlockOptions {BoundedCapacity = batchSize * outputBoundedCapacity});
            return block;
        }

        /// <summary>
        /// Connects a source and a target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Link<T>(ISourceBlock<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target)
        {
            // TODO: Remove this hack once a proper delaying transform many block is implemented.
            // (And report the bug to M$)
            if (source.GetType().FullName.Contains("Encapsulating"))
            {
                source.LinkTo(target, CreateDefaultLinkOptions());
            }
            else
            {
                source.LinkTo(target, CreateDefaultLinkOptions(), CreateDefaultFilter<T>());
                m_NullLinks.Add(() => LinkNullTarget(source));
            }
        }

        /// <summary>
        /// Connects a source and a target with the given predicate.
        /// Note: The TPL-Dataflow-Library does not throw away items that do not match the predicate.
        ///       Therefore it is your responsibility to call the LinkNullTarget() after adding all of your predicates to make
        ///       certain that non-matching items are thrown away.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        public void Link<T>(ISourceBlock<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target, Predicate<IDataflowMessage<T>> predicate)
        {
            var combinedFilter = ComposeFilterWithAnd(CreateDefaultFilter<T>(), predicate);
            source.LinkTo(target, CreateDefaultLinkOptions(), combinedFilter);
            m_NullLinks.Add(() => LinkNullTarget(source));
        }

        public void Link<T>(ISourceBlock<IDataflowMessage<T>[]> source, ITargetBlock<IDataflowMessage<T>[]> target)
        {
            source.LinkTo(target, CreateDefaultLinkOptions());
        }

        /// <summary>
        /// Links a source to a target that simply throws its messages away.
        /// Use this if you are using at least one custom link-filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        private static void LinkNullTarget<T>(ISourceBlock<IDataflowMessage<T>> source)
        {
            source.LinkTo(DataflowBlock.NullTarget<IDataflowMessage<T>>(), CreateDefaultLinkOptions(), message => message.IsBroken);
        }

        public void Link<T>(ISourceAgent<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target)
        {
            if (m_SourceAgent != null)
            {
                throw new InvalidOperationException("Source agent already registered in this network.");
            }
            source.LinkTo(target, CreateDefaultLinkOptions(), CreateDefaultFilter<T>());
            m_SourceAgent = source;
        }

        public void Link<T>(ISourceAgent<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target, Predicate<IDataflowMessage<T>> predicate)
        {
            if (m_SourceAgent != null)
            {
                throw new InvalidOperationException("Source agent already registered in this network.");
            }

            var combinedFilter = ComposeFilterWithAnd(CreateDefaultFilter<T>(), predicate);
            source.LinkTo(target, CreateDefaultLinkOptions(), combinedFilter);
            m_SourceAgent = source;
        }

        /// <summary>
        /// Saves the task of a dataflow-block in order to be able to wait for their completion.
        /// </summary>
        /// <param name="block"></param>
        private void RecordTask(IDataflowBlock block)
        {
            m_ConstituentTasks.Add(block.Completion);
        }

        /// <summary>
        /// Creates the default execution options.
        /// The default value of boundedCapacity is -1 and represents an unbounded capacity.
        /// </summary>
        /// <param name="boundedCapacity"></param>
        /// <returns></returns>
        private ExecutionDataflowBlockOptions CreateDefaultExecutionOptions(int boundedCapacity = -1)
        {
            return new ExecutionDataflowBlockOptions { CancellationToken = m_Cts.Token, BoundedCapacity = boundedCapacity };
        }

        private static DataflowLinkOptions CreateDefaultLinkOptions()
        {
            // PropagateCompletion is true because we want to automatically inform all successor blocks of the dataflow-network
            // that a block has finished its work.
            return new DataflowLinkOptions { PropagateCompletion = true };
        }

        private DataflowBlockOptions CreateDefaultBlockOptions(int boundedCapacity = -1)
        {
            return new DataflowBlockOptions { CancellationToken = m_Cts.Token, BoundedCapacity = boundedCapacity };
        }

        /// <summary>
        /// Tells all registered statistics loggers to dump their stats to the logfile.
        /// </summary>
        private void DumpStats()
        {
            foreach (var statisticsLogger in m_StatisticsLoggers)
            {
                statisticsLogger.LogStatistics();
            }
        }

        /// <summary>
        /// Creates a default LinkTo-Filter that is added to all the dataflow-blocks that are created through this dataflownetwork-class.
        /// By default we throw away all broken messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Predicate<IDataflowMessage<T>> CreateDefaultFilter<T>()
        {
            return msg => !msg.IsBroken;
        }

        /// <summary>
        /// Combines two predicates using a logical AND.
        /// This method is mainly used to combine the default filter with a user defined predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static Predicate<IDataflowMessage<T>> ComposeFilterWithAnd<T>(Predicate<IDataflowMessage<T>> left, Predicate<IDataflowMessage<T>> right)
        {
            return msg => left(msg) && right(msg);
        }

        #region IDisposable-Section

        private bool disposed = false;

        // implements IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // if this is a dispose call dispose on all state you
                // hold, and take yourself off the Finalization queue.
                if (disposing)
                {
                    /*
                    foreach (var cTasks in m_ConstituentTasks)
                    {
                        if (cTasks != null)
                            cTasks.Dispose();
                    }*/
                    
                    //if(m_Cts != null)
                    //    m_Cts.Cancel();
                    if(m_ctx != null)
                        m_ctx.Dispose();
                    m_Cts.Dispose();
                }

                // perform any custom clean-up operations
                // such as flushing the stream
                // free your own state (unmanaged objects)
                AdditionalCleanup();

                this.disposed = true;
            }

            //base.Dispose(disposing);
        }

        // finalizer simply calls Dispose(false)
        ~DataflowNetwork()
        {
            Dispose(false);
        }

        // some custom cleanup logic
        private void AdditionalCleanup()
        {
            // this method should not allocate or take locks, unless
            // absolutely needed for security or correctness reasons.
            // since it is called during finalization, it is subject to
            // all of the restrictions on finalizers above.
//            DumpStats();
            //m_LogAgent.Complete();
            m_StatisticsLoggers.Clear();
            
        }

        #endregion
        /*
        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }

            m_ctx.Dispose();
            m_Cts.Dispose();

            m_Disposed = true;
        }

        #endregion*/
    }
}
