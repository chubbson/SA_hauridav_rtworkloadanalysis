using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;

namespace WorkLoadSampler
{
    class NetMqSubscriber
    {


        static void Subscriber(NetMQContext context)
        {
            using (var serverSocket = context.CreateSubscriberSocket())
            {
                serverSocket.Subscribe(new byte[0]); //.Subscribe("tcp://*:5555");
                for (int i = 0; i <= 8; i++)
                    serverSocket.Connect(string.Format("tcp://127.0.0.1:{0}", 5555 + i));//"tcp://*:5555");

                while (true)
                {
                    string message = serverSocket.ReceiveString();

                    Console.WriteLine("Receive message {0}", message);

                    if (message == "exit")
                    {
                        break;
                    }
                }
            }
        }
    }

}
