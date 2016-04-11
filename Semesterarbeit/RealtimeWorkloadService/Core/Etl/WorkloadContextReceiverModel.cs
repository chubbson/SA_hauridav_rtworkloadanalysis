using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RealtimeWorkloadService.Crosscutting;
using ZeroMQ;

namespace RealtimeWorkloadService.Core.Etl
{
	public class WorkloadContextReceiverModel : IDisposable
	{
		private readonly Dictionary<String, Tuple<CancellationTokenSource, Task>> _feederTasks = new Dictionary<String, Tuple<CancellationTokenSource, Task>>();
		private readonly  ZContext _context;
		private readonly CancellationToken _cancellor;
		private readonly List<Task> _waitingTasks = new List<Task>(); 
		private readonly List<CancellationTokenSource> _waitingCancellationToken = new List<CancellationTokenSource>();
		private readonly Queue<string> _newKeyQueue = new Queue<string>();
		private readonly Dictionary<string, MsgContainer> _msgContainerByKey = new Dictionary<string, MsgContainer>();

		public WorkloadContextReceiverModel(CancellationToken cancellor)
		{
			_context = new ZContext();
			_cancellor = cancellor;

			Task.Factory.StartNew(() => DataListenerProxy(_context));
			Task.Factory.StartNew(() => DataBoilerProxy(_context));
		}

		// NOTE: Leave out the finalizer altogether if this class doesn't 
		// own unmanaged resources itself, but leave the other methods
		// exactly as they are. 
        ~WorkloadContextReceiverModel()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		public ZContext GetContext()
		{
			return _context; 
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
			if (disposing)
			{
				try
				{
					var keys = _feederTasks.Keys.ToList();
					foreach (var key in keys)
						RemoveTask(key);
					Task.WaitAll(_waitingTasks.ToArray());
				}
				catch (AggregateException e)
				{
					Console.WriteLine("\nAggregateException thrown with the following inner exceptions:");
					// Display information about each exception. 
					foreach (var v in e.InnerExceptions)
					{
					    var exception = v as TaskCanceledException;
					    if (exception != null)
							Console.WriteLine("   TaskCanceledException: Task {0}",
											  exception.Task.Id);
						else
							Console.WriteLine("   Exception: {0}", v.GetType().Name);
					}
				    Console.WriteLine();
				}
				finally
				{
					foreach (var wct in _waitingCancellationToken)
						wct.Dispose();
					_waitingCancellationToken.Clear();
					_waitingTasks.Clear();
					_newKeyQueue.Clear();
                }
			}
			_context.Dispose();
			// free native resources if there are any.
		}

		private void DataListenerProxy(ZContext ctx)
		{
			using (var xsubsocket = new ZSocket(ctx, ZSocketType.XSUB))
			using (var xpubsocket = new ZSocket(ctx, ZSocketType.XPUB))
			using (var listener = new ZSocket(ctx, ZSocketType.PAIR))
			{
				new Thread(() => StatisticListenerTask(ctx)).Start();

				// Frontend xsubsocket is where the publisher node(s!) sending data
				string localhost = "tcp://*:5560";
				Console.WriteLine("I: Connecting to {0}", localhost);
				xsubsocket.Bind(localhost);
				xpubsocket.Bind("inproc://databoiler");
				listener.Bind("inproc://listener");

				ZError error;
				if (!ZContext.Proxy(xsubsocket, xpubsocket, listener, out error))
				{	
					Console.Write("error: {0}", error);
					if (error == ZError.ETERM)
						return;
					throw new ZException(error);
				}
				// some error handling
			}
		}

		private void DataBoilerProxy(ZContext ctx)
		{
			using (var sub = new ZSocket(ctx, ZSocketType.SUB))
			using (var pub = new ZSocket(ctx, ZSocketType.PUB)) // dont needed anymore
			{
				sub.Connect("inproc://databoiler");
				sub.SubscribeAll(); //"asdf");//All();

				
				/***//*
				ZError error;
				ZMessage msg;
				while (null != (msg = sub.ReceiveMessage(out error)))
				{
					
					msg.frame to msgcountcountainer. 
					parse by uid. 
					store current value by uid
				}
				*//***/
				pub.Bind("inproc://datafeed");

				// like syncpub
				ZError error;
				if (!ZContext.Proxy(sub, pub, out error))
				{
					Console.Write("error: {0}", error);
					if (error == ZError.ETERM)
						return;
					throw new ZException(error);
				}
				/*
				ZMessage msg;
				ZError error;
				while (true)
					if (null == (msg = sub.ReceiveMessage(out error)))
					{
						Console.Write(string.Format("error: {0}", error));
						if (error == ZError.ETERM)
							break;
						throw new ZException(error);
					}
					else
						pub.Send(msg);
						*/
			}
		}

		private void StatisticListenerTask(ZContext ctx)
		{
			using (var listener = new ZSocket(ctx, ZSocketType.PAIR))
			{
				listener.Connect("inproc://listener");

				//checking each message, if new prefix receives, creating new Taskcomponent, adding 
				while (true)
				{
					ZError error;
				    ZMessage msg;
                    if (null == (msg = listener.ReceiveMessage(out error)))
                    {
                        if (error == ZError.ETERM)
                            return; // Interrupted
                        throw new ZException(error);
                    }
				    if (msg.Count < 3)
				        continue;
				    bool doBreak = false;
                    WorkloadStatisticsContext wlStatCtx;
                    using (var groupFrame = msg[0])
                    using (var taskFrame = msg[1])
                    using (var statlogFrame = msg[2])
                    {
                        Guid groupGuid;
                        doBreak |= !Guid.TryParse(groupFrame.ReadString(), out groupGuid);
                        Guid taskGuid;
                        doBreak |= !Guid.TryParse(taskFrame.ReadString(), out taskGuid);
                        doBreak |= !WorkloadStatisticsContext.TryParse(groupGuid, taskGuid, statlogFrame.ReadString(), out wlStatCtx);
                    }
				    if (doBreak)
				        continue;

				    var key = wlStatCtx.GetKey();
                    UInt64 cnt = _msgContainerByKey.ContainsKey(key) ? _msgContainerByKey[key].NmsReceived + 1L : 1L;
                    var msgContainer = new MsgContainer(wlStatCtx, cnt);

                    if (!_msgContainerByKey.ContainsKey(key))
                        _newKeyQueue.Enqueue(key);
                    _msgContainerByKey[key] = msgContainer;
				}
			}
		}

		public MsgContainer GetMsgContainerByKey(string key)
		{
			if (string.IsNullOrEmpty(key) || !_msgContainerByKey.ContainsKey(key))
				return null;
			return (MsgContainer)_msgContainerByKey[key].Clone();
		}

		public void AddTask()
		{
			var guidgroup = Guid.NewGuid();
			var guidtask = Guid.NewGuid();
			var tnr = String.Concat(Guid.NewGuid(), "_", Guid.NewGuid()); // Guid.NewGuid().ToString();
			var cts = new CancellationTokenSource();
			_feederTasks[tnr] =
				new Tuple<CancellationTokenSource, Task>(cts,
					Task.Factory.StartNew(
						() => 
							StartDataFeeder(_context, guidgroup, guidtask, tnr, "tcp://127.0.0.1:5560", new List<CancellationToken>() {_cancellor, cts.Token }), cts.Token));
		}

		public bool RemoveTask(string key)
		{
			if (!_feederTasks.ContainsKey(key) && !_msgContainerByKey.ContainsKey(key))// _msgWorkloadStatisticsByKey.ContainsKey(key))
				return false;

			if (_feederTasks.ContainsKey(key))
			{
				var ft = _feederTasks[key];
				_feederTasks.Remove(key);
				ft.Item1.Cancel();
				_waitingTasks.Add(ft.Item2);
				_waitingCancellationToken.Add(ft.Item1);
			}
			if (_msgContainerByKey.ContainsKey(key))
				_msgContainerByKey.Remove(key);
            return true; 
		}

		public bool TryGettingQueuedOrdNr(out string ordNr)//(out OrdinalNumbersType ordNr)
		{
			ordNr = string.Empty;//= OrdinalNumbersType.None;
			var result = _newKeyQueue.Count > 0;
			if (result) 
				ordNr = _newKeyQueue.Dequeue();
			
			return result; 
		}

		private void StartDataFeeder(ZContext ctx, Guid guidgroup, Guid guidtask, string scapt, string endpoint, List<CancellationToken> ctlist)
		{
			using (var pubsender = new ZSocket(ctx, ZSocketType.PUB))
			{
				pubsender.Connect(endpoint);//"inproc://datafeed");

				var rnd = new Random();
				while (!ctlist.Any(e => e.IsCancellationRequested))
				{
					var i = rnd.Next(150, 10000);
					var wsv = new WorkloadStatisticsContext(guidgroup, guidtask, i, i + rnd.Next(-50, 375), i + rnd.Next(-100, 200), i, scapt);
					var m = wsv.ToString();
                    ZMessage msg = new ZMessage();
                    msg.Add(new ZFrame(guidgroup.ToString()));
                    msg.Add(new ZFrame(guidtask.ToString()));
                    msg.Add(new ZFrame(m));
				    pubsender.SendMessage(msg);
					Thread.Sleep(rnd.Next(10, 75));
				}
			}
		}
	}
}
