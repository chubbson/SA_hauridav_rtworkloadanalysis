using System;
using System.Threading;
using Dfn.Etl.Core;
using RealtimeWorkloadService.Core.Etl;
using RealtimeWorkloadService.Crosscutting;
using ZeroMQ;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    public abstract class WorkloadStatisticsBase : IWorkloadStatistics, IDisposable
    {
        //private readonly string m_Title;
        private readonly DataflowNetworkConstituent _constituent;
        private WorkloadContextSenderModel _workloadContextSender; 
        private readonly int _boundedCapacity;
        private bool _disposed = false; // to detect redundant calls
        protected readonly Guid TaskGuid;
        protected readonly Guid GroupGuid;

        // Constructor
        private WorkloadStatisticsBase() {}
        protected WorkloadStatisticsBase(Guid groupguid, ZContext ctx, DataflowNetworkConstituent constituent, int boundedCapacity, CancellationToken ct)
            :this()
        {
            _constituent = constituent;
            _boundedCapacity = boundedCapacity;
            TaskGuid = Guid.NewGuid();
            GroupGuid = groupguid;
            _workloadContextSender = new WorkloadContextSenderModel(ctx);
            ct.Register(() => _workloadContextSender.Dispose());
        }

        public void Send(WorkloadStatisticsContext wsctx)
        {
            if(_workloadContextSender != null)
                _workloadContextSender.Send(wsctx);
        }

        public int GetBoundedCapacity()
        {
            return _boundedCapacity;
        }

        //public void LoadStatistics()
        //{
        //    string stats = StatisticsHelper.FormatLogMessage(m_Title, Interlocked.Read(ref NumProcessedMessages), Interlocked.Read(ref NumBrokenMessages));
        //    m_LogAgent.LogInfo(_constituent, m_Title, stats);
        //}

        // implements IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // if this is a dispose call dispose on all state you
                // hold, and take yourself off the Finalization queue.
                if (disposing)
                {
                    if (_workloadContextSender != null)
                    {
                        _workloadContextSender.Dispose();
                        _workloadContextSender = null;
                    }
                }

                // free your own state (unmanaged code)
                AdditionalCleanup();

                _disposed = true;
            }
        }

        //finalizer simply calls Dispose(false)
        ~WorkloadStatisticsBase()
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
        }
    }
}
