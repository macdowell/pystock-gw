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
                /*
                            Console.CancelKeyPress += (sender, e) =>
                            {
                                e.Cancel = true;
                                gateway.Stop();
                            };
                */
                Console.ReadLine();
                gateway.Stop();

                gateway.WaitTerminate();
                Console.WriteLine("The program ended. Press ENTER to exit...");
                Console.ReadLine();
            }
        }
    }
}
