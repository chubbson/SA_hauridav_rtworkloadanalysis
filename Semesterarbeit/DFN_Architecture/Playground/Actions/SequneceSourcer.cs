using System;
using System.Collections.Generic;
using System.IO;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    public sealed class SequenceSourcer : ISource<Tuple<UInt64, UInt64>>
    {
        private readonly string m_InputFolder;
        private readonly string m_FilePattern;
        private readonly SearchOption m_SearchOption;
        private UInt64 m_SequenceCnt;
        private UInt64 m_MaxCount; 

        public SequenceSourcer(UInt64 maxCount = 100)
        {
            m_MaxCount = maxCount;
        }

        public string Title
        {
            get { return string.Format("Generate Numbers, max count = {0}", m_MaxCount); }
        }

        public IEnumerable<Tuple<UInt64, UInt64>> Pull()
        {
/*
            using (var ctx = ZmqContext.Create())
            {
                using (var socket = ctx.CreateSocket(SocketType.PUB))
                {
                    socket.Bind("tcp://127.0.0.1:5000");

                    for (UInt32 i = 0; i < m_MaxCount; i++)
                    {
                        m_SequenceCnt++;
                        yield return new Tuple<UInt64, UInt64>(m_SequenceCnt, i*10);
                        var s = "";

                        var msg = String.Format("Publishing - msg: {0}, val: {1}", i,i*10);
                        Console.WriteLine(msg);
                      socket.Send(msg, Encoding.UTF8);
                    }
                }
            }
*/
/*
            using (NetMQContext context = NetMQContext.Create())
            {
                using (NetMQSocket clientSocket = context.CreatePublisherSocket()) //  CreateRequestSocket())
                {
                    clientSocket.Bind("tcp://127.0.0.1:5555");
 */
                    for (UInt32 i = 0; i < m_MaxCount; i++)
                    {
                        m_SequenceCnt++;
/*
                        var msg = String.Format("Publishing - msg: {0}, val: {1}", i, i * 10);
                        Console.WriteLine(msg);
                        clientSocket.Send(msg);
*/
            yield return new Tuple<UInt64, UInt64>(m_SequenceCnt, i * 10);
                    }
/*                }
            }
 */
        }
    }
    /*
            static void Publisher(NetMQContext context)
            {
                using (NetMQSocket clientSocket = context.CreatePublisherSocket())//  CreateRequestSocket())
                {
                    // clientSocket.Connect("tcp://127.0.0.1:5555");
                    clientSocket.Bind("tcp://127.0.0.1:5555");
                    
                    UInt32 m_MaxCount = UInt32.MaxValue;
                    UInt32 m_SequenceCnt = 0;
                    for (UInt32 i = 0; i <= m_MaxCount; i++)
                    {
                        m_SequenceCnt++;

                        var msg = i != m_MaxCount
                            ? String.Format("msg: {0}, val: {1}", i, i)
                            : "exit";
                        //socket.Send(msg);// SendMessage(m); //s (msg, Encoding.UTF8));
                    }
                }
            }
    */
}
