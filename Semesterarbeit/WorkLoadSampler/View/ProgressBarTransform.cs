using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using RealtimeWorkloadService.Core.Etl;
using RealtimeWorkloadService.Crosscutting;

namespace WorkLoadSampler.View
{
    public partial class ProgressBarTransform : UserControl
    {
        private int _udtCnt;
        private int _addPerfExCnt; 
        public int MsgCnt;
        public int IncCnt;
        public int OutnCnt;
        public UInt64 MaxNms;
        public int MaxIncCnt;
        public int MaxOutnCnt;
        public string Title;
        private CancellationTokenSource _mCts;
        private string _mSubscriber;
        private WorkloadContextReceiverModel _workloadContextReceiver; 

        private ProgressBarTransform()
        {
            InitializeComponent();
        }

        public ProgressBarTransform(WorkloadContextReceiverModel workloadContextReceiver, string subscriber)
            :this()
        {
            Init(workloadContextReceiver, subscriber);
        }

        private void Init(WorkloadContextReceiverModel workloadContextReceiver, string subscriber)
        {
            Name = subscriber;
            MsgCnt = 0;
            IncCnt = 0;
            OutnCnt = 0;

            MaxIncCnt = 0;
            MaxOutnCnt = 0;
            MaxNms = 0;

            upgNms.Maximum = 100000;
            upgIncMsg.Maximum = 100000;
            upgOutMsg.Maximum = 100000;

            _workloadContextReceiver = workloadContextReceiver;
            //m_endpoint = endpoint;
            _mSubscriber = subscriber;
            //m_cancellor = cancellor;
            _udtCnt = 0;
        }

        public IEnumerable<String> GetSubscribers()
        {
            return string.IsNullOrEmpty(_mSubscriber) 
                ? new List<string>() 
                : new List<string>() { _mSubscriber };
        }

        public void StartSubscriber()
        {
            if (_mCts != null)
                return; 
            _mCts = new CancellationTokenSource();
//			m_cancellor.Register(() => StopSubscriber());
    //		m_subscriberTask = Task.Factory.StartNew(() => Subscriber(m_ctx, m_endpoint, GetSubscribers(), m_cancellor, m_cts.Token), m_cts.Token);
            timer.Start();
            label1.Text = "sub started";
        }

        public void StopSubscriber()
        {
            if (_mCts != null)
            {
                _mCts.Cancel();
                //m_subscriberTask.Wait();
                label1.Text = "sub stopped";
                timer.Stop();
                _mCts.Dispose();
                _mCts = null; 
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var msgContainer = _workloadContextReceiver.GetMsgContainerByKey(_mSubscriber);
            UpdateCounter(msgContainer);
            RefreshCtrl();
        }

        public void RefreshCtrl()
        {
            label2.Text = _addPerfExCnt.ToString();
            label3.Text = _udtCnt.ToString();

            upgIncMsg.Maximum = MaxIncCnt;
            upgOutMsg.Maximum = MaxOutnCnt;

            upgNms.Value = Math.Abs(MsgCnt) % (upgNms.Maximum == 0 ? 1 : upgNms.Maximum+1);
            upgIncMsg.Value = Math.Abs(IncCnt) % (upgIncMsg.Maximum == 0 ? 1 : upgIncMsg.Maximum+1); // < 0 ? 0 : IncCnt;
            upgOutMsg.Value = Math.Abs(OutnCnt) % (upgOutMsg.Maximum == 0 ? 1 : upgOutMsg.Maximum+1); // < 0 ? 0 : OutnCnt;

            ulblinc.Text = IncCnt.ToString();
            ulblOut.Text = OutnCnt.ToString();
            ulblNMS.Text = MsgCnt.ToString();
            ulblTitle.Text = Title;

            ulblincmax.Text = MaxIncCnt.ToString();
            ulbloutmax.Text = MaxOutnCnt.ToString();
            ulblnmsmax.Text = MaxNms.ToString();
        }

        /*
        private void Subscriber(ZContext ctx, string endpoint, IEnumerable<string> subscription, CancellationToken ctParent, CancellationToken ct)
        {
            using (var subsct = ZSocket.Create(ctx, ZSocketType.SUB))
            {
                subsct.Connect(endpoint);
                foreach (var subs in subscription)
                    subsct.Subscribe(subs);

                var msgi = 0;
                
                while (!ct.IsCancellationRequested && !ctParent.IsCancellationRequested)
                {
                    ZError error;
                    ZFrame frm;
                    if (null == (frm = subsct.ReceiveFrame(out error)))
                    {
                        if (error == ZError.ETERM)
                            return; // Interrupted
                        throw new ZException(error);
                    }
                    else
                    {
                        using (frm)
                        {
                            var s = frm.ToString();//Encoding.UTF8);
                            //##Todocheck UpdateCounter(new MsgContainer(s, ++msgi));
                        }
                    }
                }
            }
        }
        */

        public void UpdateCounter(MsgContainer msgCont)
        {
            if (msgCont == null || MaxNms == msgCont.NmsReceived)
                return;
            _udtCnt++;
            MaxNms = msgCont.NmsReceived;
            // MsgCnt = msgCont.NmsReceived;// vi;//++;// = vi;

            var nmsMsg = msgCont.WorkloadStatistics.NumberOfMessagesSeen;
            var incMsg = msgCont.WorkloadStatistics.IncQueueMsgCnt;
            var outMsg = msgCont.WorkloadStatistics.OutQueueMsgCnt;

            MsgCnt = nmsMsg;
            if (incMsg > MaxIncCnt)
                MaxIncCnt = incMsg;
            if (outMsg > MaxOutnCnt)
                MaxOutnCnt = outMsg;

            IncCnt = incMsg;
            OutnCnt = outMsg;

            Title = msgCont.WorkloadStatistics.Context; // msgCont.TaskMsg; // taskMsg;// message.Split("Title: ".ToCharArray(),21)[20];

            var pcv = incMsg <= 0
                        ? 0M
                        : incMsg >= MaxIncCnt
                            ? 100M
                            : 100M / MaxIncCnt * incMsg;

            var pcv1 = outMsg <= 0
                        ? 0M
                        : outMsg >= MaxOutnCnt
                            ? 100M
                            : 100M / MaxOutnCnt * outMsg;

            try
            {
                perfChart.AddValue(pcv);
                perfChart1.AddValue(pcv1);
            }
            catch (Exception)
            {
                //throw;
                _addPerfExCnt++;
            }
        }

        


        private void ultraButton1_Click(object sender, EventArgs e)
        {
            StartSubscriber();
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            StopSubscriber();
        }
    }


}
