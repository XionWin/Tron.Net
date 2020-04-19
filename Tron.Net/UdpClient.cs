using System;
using System.Net;

namespace Tron.Net
{
    public delegate void OnDataReceivedHandler(byte[] data);
    public class UdpClient
    {
        public IPEndPoint EndPoint { get; set; }
        System.Net.Sockets.UdpClient _snd_socket = new System.Net.Sockets.UdpClient();

        private OnDataReceivedHandler OnDataReceived;

        public UdpClient(OnDataReceivedHandler OnDataReceived)
        {
            System.Net.Sockets.UdpClient rec_socket = new System.Net.Sockets.UdpClient(8815);
            var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8815);
            this.OnDataReceived += OnDataReceived;
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(
                   (o) =>
                   {
                       while (true)
                       {
                           byte[] bytes = rec_socket.Receive(ref endPoint);
                           this.OnDataReceived?.Invoke(bytes);
                           //System.Threading.Thread.Sleep(1);
                       }
                   }
               ));
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.EndPoint = endPoint;
            _snd_socket.Connect(this.EndPoint);
        }

        public void Send(byte[] data)
        {
            _snd_socket.Send(data, data.Length);
        }
    }
}
