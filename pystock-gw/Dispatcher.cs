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
        private readonly string inproc;
        private readonly string endpoint;

        private readonly QueueDevice device;
        private readonly NetMQContext context;
        
        private List<Task> workerList = new List<Task>();
        private CancellationTokenSource cancellationToken;

        private class DispatcherState
        {
            public DispatcherState(CancellationToken cancellationToken)
            {
                this.Token = cancellationToken;
            }

            public CancellationToken Token { get; private set; }
        }

        public Dispatcher(NetMQContext context, string endpoint, int count=0x05)
        {
            this.endpoint = endpoint;
            this.workerList.Capacity = count;
            this.inproc = "inproc://" + Guid.NewGuid().ToString();

            this.context = context;
            this.device = new QueueDevice(this.context, endpoint, this.inproc, DeviceMode.Threaded);
        }

        public bool IsRunning
        {
            get { return this.cancellationToken == null; }
        }

        public void Stop()
        {
            if (this.cancellationToken == null)
                return;

            // Stop queue device (Must be finished first)
            this.device.Stop();

            this.cancellationToken.Cancel();
            Task.WaitAll(this.workerList.ToArray());
            
            this.cancellationToken = null;
        }

        public void Start()
        {
            if (this.cancellationToken != null)
                return;

            this.cancellationToken = new CancellationTokenSource();

            // Run queue device (Must be started first)
            this.device.Start();

            // Run dispatch tasks
            for (int iItem = 0; iItem < this.workerList.Capacity; iItem++)
            {
                this.workerList.Add(Task.Factory.StartNew(() =>
                    this.Run(new DispatcherState(this.cancellationToken.Token)), TaskCreationOptions.LongRunning));
            }
        }

        private void Run(DispatcherState dispatcherState)
        {
            try
            {
                using (var socket = this.context.CreateDealerSocket())
                {
                    socket.Options.Identity = Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());
                    socket.Connect(this.inproc);
                    socket.ReceiveReady += this.EventHandler_OnReceiveReady;

                    while (!dispatcherState.Token.IsCancellationRequested)
                        socket.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Dispatcher worker exception: " + ex.Message);
            }
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
                Console.WriteLine("Client id: " + request[0].ConvertToString());

                NetMQMessage response = new NetMQMessage();
                response.Append(request[0].ToByteArray());
                response.Append(request[1].ToByteArray());
                return response;
            });

            return result;
        }
    }
}
