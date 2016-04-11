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
    /// Represents a decorator that enhances a many-to-one transformation with exception handling facilities.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public sealed class WorkloadStatisticsDecoratorTransformationBatched<TIn, TOut> : WorkloadStatisticsBase,  ITransformationBatchedFunctor<TIn, TOut>
    {
        private readonly ITransformationBatchedFunctor<TIn, TOut> m_DecoratedTransformationBatched;
        private readonly Func<int> m_InMsgFunc;
        private Func<int> m_OutMsgFunc;
        private int m_nms;
        private bool disposed;

        public WorkloadStatisticsDecoratorTransformationBatched(Guid groupGuid, ZContext ctx, ITransformationBatchedFunctor<TIn, TOut> decoratedTransformationBatched, int boundedCapacity, Func<int> incMsg, Func<int> outMsg, CancellationToken ct)
            :base(groupGuid, ctx, DataflowNetworkConstituent.TransformationBatched, boundedCapacity, ct)
        {
            m_DecoratedTransformationBatched = decoratedTransformationBatched;
            m_InMsgFunc = incMsg;
            m_OutMsgFunc = outMsg;
            m_nms = 0;
        }

        public string Title
        {
            get
            {
                return m_DecoratedTransformationBatched.Title;
            }
        }

        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn>[] items)
        {
            IDataflowMessage<TOut> result = m_DecoratedTransformationBatched.Transform(items);

            //var s = (Imis.Etl.Crosscutting.Logging.LogDecoratorTransformationBatched<TIn, TOut>)m_DecoratedTransformationBatched;
            //var nms = s.NumMessagesSeen;
            //var nms = -1;
            m_nms = 0;
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


    /*
        public IDataflowMessage<TOut> Transform(IDataflowMessage<TIn>[] items)
        {
            Interlocked.Increment(ref NumProcessedMessages);
            IDataflowMessage<TOut> result = m_DecoratedTransformationBatched.Transform(items);

            if (result.IsBroken)
            {
                Interlocked.Increment(ref NumBrokenMessages);
            }
            return result;
        }
    }
     */
}
