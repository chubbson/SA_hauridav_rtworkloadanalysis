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
    /// Represents a decorator that enhances a given source with exception handling facilities.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public class WorkloadStatisticsDecoratorSource<TOut> : WorkloadStatisticsBase, ISourceFunctor<TOut>
    {
        private readonly ISourceFunctor<TOut> m_DecoratedSource;
        private Func<int> m_OutMsgFunc;
        private int m_nms;
        private bool disposed = false;

        public WorkloadStatisticsDecoratorSource(Guid groupingGuid, ZContext ctx, ISourceFunctor<TOut> decoratedSource, int boundedCapacity, Func<int> outMsgCnt, CancellationToken ct)
            :base(groupingGuid, ctx, DataflowNetworkConstituent.Source, boundedCapacity, ct)
        {
            m_DecoratedSource = decoratedSource;
            m_OutMsgFunc = outMsgCnt;
            m_nms = 0; 
        }

        public string Title
        {
            get
            {
                return m_DecoratedSource.Title;
            }
        }

        public IEnumerable<IDataflowMessage<TOut>> Pull()
        {
            // Unfortunately, this guy can't do much, because he will either have to enumerate the enumerable
            // (thereby materializing it) or just delay the computation further as is the case here.
            //// return m_DecoratedSource.Pull().Select(item => new DefaultDataflowMessage<TOut>(item.Data).WithTitle(Title)); 
            m_nms = 0;
            var res = m_DecoratedSource.Pull().Select(item =>
            {
                var dfm = new DefaultDataflowMessage<TOut>(item.Data).WithTitle(Title);
                m_nms++;
                Send(new WorkloadStatisticsContext(this.GroupGuid, this.TaskGuid, m_nms, -1, m_OutMsgFunc(), GetBoundedCapacity(), Title));
                return dfm;
            });

            return res;
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
