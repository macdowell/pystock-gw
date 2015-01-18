using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ.Devices;
using NetMQ.Sockets;
using NetMQ.zmq;
using NetMQ;

namespace PyStock
{
    class ConsoleApplication    
    {
        static void Main(string[] args)
        {
            using(Gateway gateway = new Gateway("tcp://*:5555", "tcp://*:5556", 10))
            {
                gateway.Start();
                ManualResetEvent resetEvent = new ManualResetEvent(false);

                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    resetEvent.Set();
                };

                Console.WriteLine("Press Ctrl + C to stop...");
                
                resetEvent.WaitOne();
                gateway.Stop();

                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
        }
    }
}
