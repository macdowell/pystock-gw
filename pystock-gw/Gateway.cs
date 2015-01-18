using System;

using System.Threading;

using System.Collections.Generic;

using NetMQ;
using NetMQ.Devices;

namespace PyStock
{
    class Gateway : IDisposable
    {
        private readonly Dispatcher dispather;
        private readonly NetMQContext zmq_context = NetMQContext.Create();

        public Gateway(string dispatcher_endpoint, string pulisher_endpoint, 
            int dispatcher_count = 0x05, int pulisher_count=0x05)
        {
            this.dispather = new Dispatcher(this.zmq_context, dispatcher_endpoint);
        }

        public void Stop()
        {
            this.dispather.Stop();
        }

        public void Start()
        {
            this.dispather.Start();
        }

        public void Dispose()
        {
            this.zmq_context.Dispose();
        }    
    }
}
