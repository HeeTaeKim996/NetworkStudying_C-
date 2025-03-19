using System.Net;
using System.Net.Sockets;


namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            clientSocket.Connect(listenEndPoint);

            using(FileStream fsOutput = new FileStream("1_Copy.webp", FileMode.Create))
            {
                byte[] buffer = new byte[500];
                int RecvLength = 0;

                do
                {
                    RecvLength = clientSocket.Receive(buffer);
                    fsOutput.Write(buffer, 0, RecvLength);

                    Console.WriteLine($"Client_RecvLength : {RecvLength}");
                } while (RecvLength > 0);

                fsOutput.Close();
            }

            clientSocket.Close();
        }

    }
}