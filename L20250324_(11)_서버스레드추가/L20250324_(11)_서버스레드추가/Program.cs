using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Client 
{
    class Program
    {
        private static Socket clientSocket;

        static void Main(string[] args)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            clientSocket.Connect(serverEndPoint);


            Thread sendThread = new Thread(new ThreadStart(SendThread));
            sendThread.IsBackground = true;
            Thread recvThread = new Thread(new ThreadStart(RecvThread));
            recvThread.IsBackground = true;

            sendThread.Start();
            recvThread.Start();


            sendThread.Join();
            recvThread.Join();


            clientSocket.Close();
        }

        private static void SendThread()
        {
            while (true)
            {
                //Console.Write("채팅 : ");
                string inputString = Console.ReadLine();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(inputString);

                int headerInt = 2;
                byte[] headerBuffer = new byte[headerInt];
                ushort packetLength = (ushort)dataBuffer.Length;

                headerBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packetLength));

                byte[] sendBuffer = new byte[headerInt + packetLength];
                Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
                Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);

                clientSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
            }     
        }

        private static void RecvThread()
        {
            while (true)
            {
                int headerInt = 2;
                byte[] headerBuffer = new byte[headerInt];

                int RecvLength = clientSocket.Receive(headerBuffer, headerInt, SocketFlags.None);
                ushort packetLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));

                byte[] recvBuffer = new byte[packetLength];
                RecvLength = clientSocket.Receive(recvBuffer, packetLength, SocketFlags.None);

                string recvString = Encoding.UTF8.GetString(recvBuffer);
                Console.WriteLine(recvString);
            }
        }

    }
}
