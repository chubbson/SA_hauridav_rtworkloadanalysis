using System;
using RealtimeWorkloadService.Crosscutting;
using ZeroMQ;

namespace RealtimeWorkloadService.Core.Etl
{
	public class WorkloadContextSenderModel : IDisposable
	{
	    private ZSocket _pubSocket;
	    private bool _disposed = false;

	    public WorkloadContextSenderModel(ZContext ctx)
		{
            _pubSocket = new ZSocket(ctx, ZSocketType.PUB);
		    _pubSocket.Connect("tcp://127.0.0.1:5560");
		}

		// NOTE: Leave out the finalizer altogether if this class doesn't 
		// own unmanaged resources itself, but leave the other methods
		// exactly as they are. 
        ~WorkloadContextSenderModel()
		{
			// Finalizer calls Dispose(false)
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

		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
		    if (!_disposed)
		    {
		        // if this is a dispose call dispose on all state you
		        // hold, and take yourself off the Finalization queue.
		        if (disposing)
		        {
		            if (_pubSocket != null)
		            {
		                _pubSocket.Dispose();
		                _pubSocket = null;
		            }
		        }

		        // free your own state (unmanaged objects)
		        AdditionalCleanup();
		        _disposed = true;
		    }
		}

	    /// <summary>
	    /// not threadsave! 1 socket created by constructor
	    /// http://zguide.zeromq.org/page:all
	    /// - Do not try to use the same socket from multiple threads.
	    /// </summary>
	    /// <param name="wsCtx"></param>
	    public void Send(WorkloadStatisticsContext wsCtx)
	    {
            var msg = new ZMessage();
            msg.Add(new ZFrame(wsCtx.GroupGuid.ToString()));
            msg.Add(new ZFrame(wsCtx.TaskGuid.ToString()));
            msg.Add(new ZFrame(wsCtx.ToString()));
	        _pubSocket.Send(msg);
	    }
	}
}
