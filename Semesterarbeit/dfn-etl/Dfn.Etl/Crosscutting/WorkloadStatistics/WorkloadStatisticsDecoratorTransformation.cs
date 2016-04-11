//using NetMQ.Sockets;
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
    /// Represents a decorator that enhances a transformation with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class WorkloadStatisticsDecoratorTransformation<TIn, TOut> : WorkloadStatisticsBase, ITransformationFunctor<TIn, TOut>
    {
        private readonly ITransformationFunctor<TIn, TOut> m_DecoratedTransformation;
        private readonly Func<int> m_InMsgFunc;
        private readonly Func<int> m_OutMsgFunc;
        private int m_nms;
        private bool disposed;

        public WorkloadStatisticsDecoratorTransformation(Guid groupGuid, ZContext ctx, ITransformationFunctor<TIn, TOut> decoratedTransformation, int boundedCapacity, Func<int> incMsg, Func<int> outMsg, CancellationToken ct)
            :base(groupGuid, ctx, DataflowNetworkConstituent.Transformation, boundedCapacity, ct)
        {
            m_DecoratedTransformation = decoratedTransformation;
            m_InMsgFunc = incMsg;
            m_OutMsgFunc = outMsg;
            m_nms = 0;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTransformation.Title;
            }
        }

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn> item)
        {
            IDataflowMessage<TOut> result = m_DecoratedTransformation.Transform(item);
            m_nms++;
            Send(new WorkloadStatisticsContext(this.GroupGuid, this.TaskGuid, m_nms, m_InMsgFunc(), m_OutMsgFunc(), GetBoundedCapacity(), Title));
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
