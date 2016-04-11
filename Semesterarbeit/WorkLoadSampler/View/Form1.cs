using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;
using RealtimeWorkloadService.Core.Etl;

namespace WorkLoadSampler.View
{
    public partial class Form1 : Form
    {
	    private readonly Dictionary<String, ProgressBarTransform> _pbtdict = new Dictionary<string, ProgressBarTransform>();
	    private CancellationTokenSource _cts;
	    private WorkloadContextReceiverModel _workloadContextReceiverModel;
//	    private Queue<OrdinalNumbersType> _newOrdNrQueue = new Queue<OrdinalNumbersType>();

		public Form1()
        {
            InitializeComponent();
			Init();
        }

	    private void Init()
	    {
			_cts = new CancellationTokenSource();
			_workloadContextReceiverModel = new WorkloadContextReceiverModel(_cts.Token);
			timer1.Start();
	    }
		
        /*
         void Subscriber()
        {
            using (NetMQContext context = NetMQContext.Create())
            {
                using (var serverSocket = context.CreateSubscriberSocket())
                {
                    serverSocket.Subscribe(new byte[0]); //.Subscribe("tcp://*:5555");
                    //for (int i = 0; i <= 8; i++)
                        serverSocket.Connect(string.Format("tcp://127.0.0.1:{0}", 5556));//"tcp://*:5555");

                    while (true)
                    {
                        //var m = string.Format("NMS: {0}|BC: {1}|IncMsg: {2}|OutMsg: {3}|Title: {4}", -1, bc, m_InMsgFunc(), -1, Title);
                        string message = serverSocket.ReceiveString();
                        var splitted = message.Split("Title: ".ToCharArray());
                        splitted = message.Split('|');
                        var nms = splitted[0].Split(' ')[1];
                        var bc = splitted[1].Split(' ')[1];
                        var incmsg = splitted[2].Split(' ')[1];
                        var outmsg = splitted[3].Split(' ')[1];
                        var nmsBuffer = splitted[4];

                        int vi = int.Parse(nms);

                        progVal = vi;
                        prog2Val = int.Parse(incmsg);
                        //if (splitted[0] == "NMS:") ;
                        //message.

                        Console.WriteLine("Receive message {0}", message);

                        if (message == "exit")
                        {
                            break;
                        }
                    }
                }
            }
        }*/

	    private void RemoveAllUiProgressComponent()
	    {
			List<string> removableTabPage = new List<string>();
		    var tabpageEnumerator = tabControl1.TabPages.GetEnumerator();
		    while (tabpageEnumerator.MoveNext())
		    {
				removableTabPage.Add(((TabPage) tabpageEnumerator.Current).Name);
			    var tp = (TabPage)tabpageEnumerator.Current;
			    foreach (var flpctrl in
				    tp.Controls.Cast<object>()
					    .Where(ctrl => ctrl.GetType() == typeof (FlowLayoutPanel))
					    .Select(ctrl => (FlowLayoutPanel) ctrl)
					    .Select(flp => flp.Controls.Cast<object>().ToList())
					    .SelectMany(rmvblCtrl => rmvblCtrl))
			    {
				    RemoveUiPogressComponent((ProgressBarTransform)flpctrl);
			    }
		    }
		    foreach (var rtp in removableTabPage)
				tabControl1.TabPages.RemoveByKey(rtp);
			
	    }

	    private void RemoveUiPogressComponent(ProgressBarTransform ctrl)
	    {
		    IEnumerable<string> subs;
		    using (var pbt = ctrl)
		    {
			    subs = pbt.GetSubscribers();
			    pbt.StopSubscriber();
		    }
			foreach (var subscription in subs)
			{
				_workloadContextReceiverModel.RemoveTask(subscription);
				var pgt = _pbtdict[subscription];
				_pbtdict.Remove(subscription);
				if (pgt.IsDisposed || !pgt.Disposing)
					pgt.Dispose();
			}
		}

	    private void AddNewUiProgressComponent(string ordnr)//OrdinalNumbersType ordnr)
	    {
		    if (string.IsNullOrEmpty(ordnr))
			    return;
		    //var sOrndNrHex = ordnr;//.ToStringHex();
		    var splittedordNr = ordnr.Split('_');
		    if (splittedordNr.Count() < 2)
			    return;

		    string tbkey = string.Concat("tb", splittedordNr[0]);
			string flpKey = string.Concat("flp", splittedordNr[0]);
		    string pbtKey = ordnr; // string.Concat("pbtKey", splittedordNr[1]);

		    if (!tabControl1.TabPages.ContainsKey(tbkey))
			    tabControl1.TabPages.Add(tbkey, tbkey);
		    var tp = tabControl1.TabPages[tbkey];

			FlowLayoutPanel flp;
			if (!tp.Controls.ContainsKey(flpKey))
		    {
			    flp = new FlowLayoutPanel();
			    flp.Name = flpKey;
			    flp.AutoScroll = true;

				flp.FlowDirection = FlowDirection.TopDown;
			    flp.WrapContents = false;
				tp.Controls.Add(flp);
			    flp.Dock = DockStyle.Fill;
			    tp.Controls.Add(flp);
		    }
		    else
		    {
			    flp = (FlowLayoutPanel)tp.Controls[flpKey];
		    }

		    ProgressBarTransform pbt;
		    if (!flp.Controls.ContainsKey(pbtKey))
		    {
				pbt = new ProgressBarTransform(_workloadContextReceiverModel, pbtKey);
				flp.Controls.Add(pbt);
		    }
		    else
		    {
			    pbt = (ProgressBarTransform) flp.Controls[pbtKey];
		    }

		    if (!_pbtdict.ContainsKey(pbtKey))
		    {
			    _pbtdict[pbtKey] = pbt;
		    }
			pbt.StartSubscriber();
			//_workloadContextReceiverModel.AddSubscriber();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			//_cts.Cancel();
			timer1.Stop();
			RemoveAllUiProgressComponent();
			_workloadContextReceiverModel.Dispose();
			foreach (var val in _pbtdict)
			{
				//val.Value.StopSubscriber();
				val.Value.Parent = null;
			}
			_pbtdict.Clear();
		}

		private void ultraButton1_Click(object sender, EventArgs e)
		{
			_workloadContextReceiverModel.AddTask();// Adding Feeder
		}

		private void btnRmvFeederTask_Click(object sender, EventArgs e)
		{
			var tp = tabControl1.SelectedTab;
			FlowLayoutPanel flp = tp.Controls.Cast<object>().Where(ctrl => ctrl.GetType() == typeof (FlowLayoutPanel)).Cast<FlowLayoutPanel>().FirstOrDefault();
			if (flp == null)
				return;
			List<Object> rmvblCtrl = flp.Controls.Cast<object>().ToList();

		    if (!rmvblCtrl.Any())
		        return; 
		    
			var obj = rmvblCtrl.Last();
			RemoveUiPogressComponent((ProgressBarTransform)obj);
		}

		private void timer1_Tick_1(object sender, EventArgs e)
		{
			//OrdinalNumbersType ordNr;
			string ordNr; 
			while (_workloadContextReceiverModel != null && _workloadContextReceiverModel.TryGettingQueuedOrdNr(out ordNr))
				AddNewUiProgressComponent(ordNr);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			RemoveAllUiProgressComponent();
		}
	}
}