using System;
using System.Linq;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using NetMQ;
using RealtimeWorkloadService.Crosscutting;
using ZeroMQ;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    /// <summary>
    /// Represents a decorator that enhances a batched target with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class WorkloadStatisticsDecoratorTargetBatched<TIn> : WorkloadStatisticsBase,  ITargetBatchedFunctor<TIn>
    {
        private readonly ITargetBatchedFunctor<TIn> m_DecoratedTarget;
        private Func<int> m_InMsgFunc;
        private int m_nms;
        private bool disposed = false;

        public WorkloadStatisticsDecoratorTargetBatched(Guid groupGuid, ZContext ctx, ITargetBatchedFunctor<TIn> decoratedTarget, int boundedCapacity, Func<int> inMsgFunc, CancellationToken ct)
            : base(groupGuid, ctx, DataflowNetworkConstituent.TargetBatched, boundedCapacity, ct)
        {
            m_DecoratedTarget = decoratedTarget;
            m_InMsgFunc = inMsgFunc;
            m_nms = 0;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTarget.Title;
            }
        }

        public void Push(IDataflowMessage<TIn>[] items)
        {
            var skippedBrokenItemsArray = items.Where(item => !item.IsBroken).Select(item => item).ToArray();
            m_DecoratedTarget.Push(skippedBrokenItemsArray);

            //var itemsCnt = items.Count();
            //var validItemsCnt = skippedBrokenItemsArray.Count();
            m_nms++;
            Send(new WorkloadStatisticsContext(this.GroupGuid, this.TaskGuid, m_nms, m_InMsgFunc(), -1, GetBoundedCapacity(), Title));
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // free any disposable resources you own
                    //if (myComponent != null)
                    //    myComponent.Dispose();
                    this.disposed = true;
                }

                // perform any custom clean-up operations
                // such as flushing the stream
            }

            base.Dispose(disposing);
        }

    }
}
