using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Functors;
using RealtimeWorkloadService.Crosscutting;
using ZeroMQ;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    /// <summary>
    /// Represents a decorator that enhances a one-to-many transformation with exception handling facilities.
    /// </summary>
    public sealed class WorkloadStatisticsDecoratorTransformMany<TIn, TOut> : WorkloadStatisticsBase, ITransformManyFunctor<TIn, TOut>
    {
        private readonly ITransformManyFunctor<TIn, TOut> m_Decorated;
        private bool disposed = false;

        public WorkloadStatisticsDecoratorTransformMany(Guid groupguid, ZContext ctx, ITransformManyFunctor<TIn, TOut> decorated, int boundedCapacity, CancellationToken ct)
            :base(groupguid, ctx, DataflowNetworkConstituent.TransformMany, boundedCapacity, ct)
        {
            m_Decorated = decorated;
        }

        public string Title
        {
            get { return m_Decorated.Title; }
        }

        public IEnumerable<IDataflowMessage<TOut>> TransformMany(IDataflowMessage<TIn> item)
        {
            var result = m_Decorated.TransformMany(item).Select(c => new DefaultDataflowMessage<TOut>(c.Data).WithTitle(Title));

            //Send(new WorkloadStatisticsContext(this.TaskGuid, m_nms, m_InMsgFunc(), m_OutMsgFunc(), GetBoundedCapacity(), Title));
            return result;
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