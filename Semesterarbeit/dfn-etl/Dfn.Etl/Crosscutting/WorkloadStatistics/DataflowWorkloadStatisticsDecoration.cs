using System;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using Dfn.Etl.Core.Tasks;
using Dfn.Etl.Crosscutting.Logging;
using ZeroMQ;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    /// <summary>
    /// Provides a group of methods that decorate dataflow network with logging capabilities.
    /// </summary>
    public static class DataflowWorkloadStatisticsDecoration
    {
        public static WorkloadStatisticsDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> WithWorkloadStatistics<TInput, TInputMsg, TOutput, TOutputMsg>(this IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> decoratedSource, IIdasDataflowNetwork network, ILogAgent logAgent)
            where TInputMsg : IDataflowMessage<TInput>
            where TOutputMsg : IDataflowMessage<TOutput>
        {
            return new WorkloadStatisticsDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>(decoratedSource, network, logAgent);
        }

        public static ISourceFunctor<TOut> WithWorkloadStatistiscs<TOut>(this ISourceFunctor<TOut> decoratedSource, int boundetCapacity, Func<int> outMsgCnt, ZContext ctx, Guid groupguid, CancellationToken ct)
        {
            return new WorkloadStatisticsDecoratorSource<TOut>(groupguid, ctx, decoratedSource, boundetCapacity, outMsgCnt, ct);//(decoratedSource, boundetCapacity, outMsgCnt);
        }

        public static ITransformationFunctor<TIn, TOut> WithWorkloadStatistics<TIn, TOut>(this ITransformationFunctor<TIn, TOut> decoratedTransformation, int boundedCapacity, Func<int> incMsgCnt, Func<int> outMsgCnt, ZContext ctx, Guid groupguid, CancellationToken ct)
        {
            return new WorkloadStatisticsDecoratorTransformation<TIn, TOut>(groupguid, ctx, decoratedTransformation, boundedCapacity, incMsgCnt, outMsgCnt, ct);
        }

        public static ITransformationBatchedFunctor<TIn, TOut> WithWorkloadStatistics<TIn, TOut>(this ITransformationBatchedFunctor<TIn, TOut> decoratedTransformation, int boundedCapacity, Func<int> incMsgCnt, Func<int> outMsgCnt, ZContext ctx, Guid groupguid, CancellationToken ct)
        {
            return new  WorkloadStatisticsDecoratorTransformationBatched<TIn, TOut>(groupguid, ctx, decoratedTransformation, boundedCapacity, incMsgCnt, outMsgCnt, ct);
        }

        public static ITransformManyFunctor<TIn, TOut> WithWorkloadStatistics<TIn, TOut>(this ITransformManyFunctor<TIn, TOut> decoratedTransformation, int boundedCapacity, ZContext ctx, Guid groupguid, CancellationToken ct)
        {
            return new WorkloadStatisticsDecoratorTransformMany<TIn, TOut>(groupguid, ctx, decoratedTransformation, boundedCapacity, ct);
        }

        public static ITargetFunctor<TIn> WithWorkloadStatistics<TIn>(this ITargetFunctor<TIn> decoratedTarget, int boundedCapacity, Func<int> incMsgCnt, ZContext ctx, Guid groupguid, CancellationToken ct)
        {
            return new WorkloadStatisticsDecoratorTarget<TIn>(groupguid,ctx, decoratedTarget, boundedCapacity, incMsgCnt, ct);
        }

        public static ITargetBatchedFunctor<TIn> WithWorkloadStatistics<TIn>(this ITargetBatchedFunctor<TIn> decoratedTarget, int boundedCapacity, Func<int> incMsgCnt, ZContext ctx, Guid groupguid, CancellationToken ct)
        {
            return new WorkloadStatisticsDecoratorTargetBatched<TIn>(groupguid, ctx, decoratedTarget, boundedCapacity, incMsgCnt, ct);
        }
    }
}
