using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core.Blocks;

namespace Dfn.Etl.Core
{
    public class IdasDataflowLinkOptions : DataflowLinkOptions
    {
        /// <summary>
        /// True, if the dataflow network should wait for the target to complete before finishing.
        /// </summary>
        public bool? WaitForTargetCompletion { get; set; }
    }

    /// <summary>
    /// Represents a dataflow network that knows how to create its constituents and how to link them together.
    /// Its purpose is to simplify the creation of a dataflow network using sane default options as well as actually using it.
    /// </summary>
    public interface IIdasDataflowNetwork
    {
        BufferBlock<TOutMsg> CreateBufferBlock<TOut, TOutMsg>(DataflowBlockOptions options = null) where TOutMsg : IDataflowMessage<TOut>;
        BatchBlock<TOutMsg> CreateBatchBlock<TOut, TOutMsg>(int batchSize, GroupingDataflowBlockOptions options = null) where TOutMsg : IDataflowMessage<TOut>;

        void Link<TMsg>(ISourceBlock<TMsg> sourceBlock, ITargetBlock<TMsg> target, IdasDataflowLinkOptions linkOptions = null);

        void Start(ISourceBlock sourceBlock);
        void CancelNetwork();

        Task<bool> SendMessage<TMsg>(ITargetBlock<TMsg> source, TMsg msg);

        DataflowBlockOptions CreateDefaultBlockOptions(int boundedCapacity = -1);
        ExecutionDataflowBlockOptions CreateDefaultExecutionBlockOptions(int boundedCapacity = -1);
        GroupingDataflowBlockOptions CreateDefaultGroupingBlockOptions(int batchSize, int boundedCapacity = -1);
    }

    /// <summary>
    /// Represents a dataflow network that knows how to create its constituents and how to link them together.
    /// Its purpose is to simplify the creation of a dataflow network using sane default options as well as actually using it.
    /// </summary>
    public interface IDataflowNetwork
    {
        ISourceAgent<IDataflowMessage<TOut>> CreateSource<TOut>(ISource<TOut> source, int boundedCapacity = -1);
        BufferBlock<IDataflowMessage<TOut>> CreateBuffer<TOut>(int boundedCapacity = -1);
        TransformBlock<IDataflowMessage<TIn>, IDataflowMessage<TOut>> CreateTransform<TIn, TOut>(ITransformation<TIn, TOut> transformation, int boundedCapacity = -1);
        IPropagatorBlock<IDataflowMessage<TIn>, IDataflowMessage<TOut>> CreateDelayingTransformMany<TIn, TOut>(ITransformMany<TIn, TOut> transformation, int boundedCapacity = -1);
        ActionBlock<IDataflowMessage<TIn>> CreateTarget<TIn>(ITarget<TIn> target, int boundedCapacity = -1);
        ActionBlock<IDataflowMessage<TIn>[]> CreateTargetBatched<TIn>(ITargetBatched<TIn> target, int boundedCapacity = -1);
        BatchBlock<IDataflowMessage<TBatched>> CreateBatch<TBatched>(int batchSize, int outputBoundedCapacity = 1);
        
        void Link<T>(ISourceBlock<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target);
        void Link<T>(ISourceBlock<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target, Predicate<IDataflowMessage<T>> predicate);
        void Link<T>(ISourceBlock<IDataflowMessage<T>[]> source, ITargetBlock<IDataflowMessage<T>[]> target);
        void Link<T>(ISourceAgent<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target);
        void Link<T>(ISourceAgent<IDataflowMessage<T>> source, ITargetBlock<IDataflowMessage<T>> target, Predicate<IDataflowMessage<T>> predicate);
        void Start<T>(ISourceAgent<T> source);
    }
}