using System.Net;
using System.Net.Sockets;


namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 6000);
            serverSocket.Bind(serverEndPoint);

            byte[] buffer = new byte[1024];
            EndPoint clientEndPoint = (EndPoint)serverEndPoint;

            int RecvLength = serverSocket.ReceiveFrom(buffer, ref clientEndPoint);

            int SendLength = serverSocket.SendTo(buffer, clientEndPoint);

            serverSocket.Close();
        }
    }
}