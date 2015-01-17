using System;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;

using NetMQ;
using NetMQ.zmq;
using NetMQ.Sockets;
using NetMQ.Devices;

namespace PyStock
{
    class Dispatcher
    {
        private readonly Gateway gateway;

        private readonly string zmq_inproc;
        private readonly QueueDevice zmq_queue;
        private readonly NetMQContext zmq_context;

        private List<ManualResetEvent> workerList = new List<ManualResetEvent>();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private class DispatcherState
        {
            public DispatcherState(ManualResetEvent resetEvent, CancellationToken cancellationToken)
            {
                this.Event = resetEvent;
                this.Token = cancellationToken;
            }

            public ManualResetEvent Event { get; private set; }
            public CancellationToken Token { get; private set; }
        }

        public Dispatcher(Gateway gateway)
        {
            this.gateway = gateway;
            this.zmq_context = gateway.ZmqContext;

            this.zmq_inproc = "inproc://" + Guid.NewGuid().ToString();
            this.zmq_queue = new QueueDevice(this.zmq_context, this.gateway.DispatcherEndpoint,
                this.zmq_inproc, DeviceMode.Threaded);
        }

        public void Stop()
        {
            // Warning, this operation should always be the first!
            this.zmq_queue.Stop(); 

            this.cancellationToken.Cancel();
            WaitHandle.WaitAll(this.workerList.ToArray());
        }

        public void Start()
        {
            try
            {
                // Warning, this operation should always be the first!
                this.zmq_queue.Start();

                for (int iItem = 0; iItem < this.gateway.WorkerCount; iItem++)
                {
                    var resetEvent = new ManualResetEvent(false);
                    if (ThreadPool.QueueUserWorkItem(new WaitCallback(this.Run), 
                        new DispatcherState(resetEvent, this.cancellationToken.Token)))
                    {
                        this.workerList.Add(resetEvent);
                    }
                    else
                    {
                        this.Stop();
                        Console.WriteLine("Can't run the dispatcher workes.");
                        throw new Exception("Can't run the dispatcher workes.");
                    }
                }
            } catch(NotSupportedException)
            {
                this.zmq_queue.Stop();

                Console.WriteLine("These API's may fail when called on a non-Windows 2000 system.");
                throw;
            }            
        }

        private void Run(object state)
        {
            var dispatcherState = (DispatcherState)state;

            try
            {
                using (ResponseSocket socket = this.zmq_context.CreateResponseSocket())
                {
                    socket.Options.Identity = Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());
                    socket.Connect(this.zmq_inproc);
                    socket.ReceiveReady += this.EventHandler_OnReceiveReady;

                    while (!dispatcherState.Token.IsCancellationRequested)
                        socket.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Dispatcher worker exception: " + ex.Message);
            }

            dispatcherState.Event.Set();
        }

        private async void EventHandler_OnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket socket = e.Socket;

            var request = socket.ReceiveMessage();
            var response = await Dispatch(request);

            socket.SendMessage(response);
        }

        private async Task<NetMQMessage> Dispatch(NetMQMessage request)
        {
            var result = await Task<NetMQMessage>.Factory.StartNew(() =>
            {
                NetMQMessage response = new NetMQMessage();

                if (request.FrameCount != 0x03)
                {
                    response.Append("ERROR");
                    response.Append("Invalid message format");
                }
                return response;
            });

            return result;
        }
    }
}
