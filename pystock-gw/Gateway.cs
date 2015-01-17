using System;

using System.Threading;

using System.Collections.Generic;

using NetMQ;
using NetMQ.Devices;

namespace PyStock
{
    class Gateway : IDisposable
    {
        int worker_count = 0x4;
        bool _disposed = false;

        private string pulisher_endpoint;
        private string dispatcher_endpoint;
        
        private Dispatcher dispather;
        private readonly NetMQContext zmq_context = NetMQContext.Create();

        public Gateway(string dispatcher_endpoint, string pulisher_endpoint, int worker_count=0x04)
        {
            this.worker_count = worker_count;
            this.pulisher_endpoint = pulisher_endpoint;
            this.dispatcher_endpoint = dispatcher_endpoint;

            this.dispather = new Dispatcher(this);
        }

        ~Gateway()
        {
            this.Dispose(false);
        }

        public NetMQContext ZmqContext
        {
            get { return this.zmq_context; }
        }

        public int WorkerCount
        {
            get { return this.worker_count; }
        }

        public string PublisherEndpoint
        {
            get { return this.pulisher_endpoint; }
        }

        public string DispatcherEndpoint 
        {
            get { return this.dispatcher_endpoint; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Stop()
        {
            this.dispather.Stop();
        }

        public void Start()
        {
            this.dispather.Start();
        }

        public void WaitTerminate()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
                return;

            if (disposing)
            {
                this.dispather = null;

                this.pulisher_endpoint = null;
                this.dispatcher_endpoint = null;
            }

            this.zmq_context.Dispose();

            this._disposed = true;
        }
    }
}
