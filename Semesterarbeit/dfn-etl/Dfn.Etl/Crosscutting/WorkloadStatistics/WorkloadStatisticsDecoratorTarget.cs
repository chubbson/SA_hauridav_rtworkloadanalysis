using System;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using NetMQ;
using RealtimeWorkloadService.Crosscutting;
using ZeroMQ;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    /// <summary>
    /// Represents a decorator that enhances a target with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public sealed class WorkloadStatisticsDecoratorTarget<TIn> : WorkloadStatisticsBase, ITargetFunctor<TIn>
    {
        private readonly ITargetFunctor<TIn> m_DecoratedTarget;
        private Func<int> m_InMsgFunc;
        private int m_nms;
        private bool disposed; 

        public WorkloadStatisticsDecoratorTarget(Guid groupGuid, ZContext ctx, ITargetFunctor<TIn> decoratedTarget, int boundedCapacity, Func<int> incMsgCnt, CancellationToken ct)
            :base(groupGuid, ctx, DataflowNetworkConstituent.Target, boundedCapacity, ct)
        {
            m_DecoratedTarget = decoratedTarget;
            m_InMsgFunc = incMsgCnt;
            m_nms = 0;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTarget.Title;
            }
        }

        public void Push(IDataflowMessage<TIn> item)
        {
            if (!item.IsBroken)
            {
                m_DecoratedTarget.Push(item);
                m_nms++;
                Send(new WorkloadStatisticsContext(this.GroupGuid, this.TaskGuid, m_nms, m_InMsgFunc(), -1, GetBoundedCapacity(), Title));
            }
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
