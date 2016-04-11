using System;
using System.Threading;
using NetMQ;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    public sealed class WorkloadStatisticsZmqPublisher
    {
        private NetMQContext m_Context;
        private NetMQSocket m_PubSocket;
        public const int PortSeed = 5555;
        private int m_CurrentPort;
        private static int s_PortCntr = 0;
        private static object syncRoot = new Object();

        
        public void Init()
        {

            var m_PubSocket = GetContext().CreatePublisherSocket();
            for (int i = 1; i <= s_PortCntr; i++)
            {
                m_PubSocket.Disconnect(string.Format("tcp://127.0.0.1:{0}", PortSeed + i));
            }
            s_PortCntr = 0;
            m_CurrentPort = 0;
        }
        

        private NetMQContext GetContext()
        {
            if (m_Context == null)
            {
                m_Context = NetMQContext.Create();
            }
            return m_Context;
        }

        public int PortCnt
        {
            get { return s_PortCntr; }
        }

        public int MaxPort
        {
            get { return PortSeed + s_PortCntr; }
        }

        public int CurrentPort
        {
            get { return m_CurrentPort; }
        }

        public NetMQSocket GetPubSocket()
        {
            if (m_PubSocket == null)
            {
                lock (syncRoot)
                {
                    if (m_PubSocket == null)
                    {
                        m_PubSocket = GetContext().CreatePublisherSocket();
                        m_CurrentPort = PortSeed + Interlocked.Increment(ref s_PortCntr);
                        m_PubSocket.Bind(string.Format("tcp://127.0.0.1:{0}", m_CurrentPort));
                    }
                }
            }

            return m_PubSocket;
        }

            //using (NetMQContext context = NetMQContext.Create())
            //{
            //    using (NetMQSocket clientSocket = context.CreatePublisherSocket()) //  CreateRequestSocket())
            //    {
            //        clientSocket.Bind("tcp://127.0.0.1:5555");
            //        for (UInt32 i = 0; i < m_MaxCount; i++)
            //        {
            //            m_SequenceCnt++;
            //            var msg = String.Format("Publishing - msg: {0}, val: {1}", i, i * 10);
            //            Console.WriteLine(msg);
            //            clientSocket.Send(msg);
            //            yield return new Tuple<UInt64, UInt64>(m_SequenceCnt, i * 10);
            //        }
            //    }
            //}
    }
}
