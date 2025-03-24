using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    class Program
    {
        private static Socket serverSocket;
        private static List<Socket> clientSockets = new List<Socket>();
        private static Object lock_ClientSockets = new Object();
        //static List<Thread> threadManager = new List<Thread>();

        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            serverSocket.Bind(serverEndPoint);
            serverSocket.Listen(10);


            Thread acceptThread = new Thread(new ThreadStart(AcceptThread));
            acceptThread.IsBackground = true;
            acceptThread.Start();

            acceptThread.Join();

            serverSocket.Close();
        }

        private static void AcceptThread()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                lock (lock_ClientSockets)
                {
                    clientSockets.Add(clientSocket);
                }
                Console.WriteLine($"Connected : {clientSocket.RemoteEndPoint}");

                Thread clientWorkThread = new Thread(new ParameterizedThreadStart(ClientWorkThread));
                clientWorkThread.Start(clientSocket);
                //threadManager.Add(workThread);
            }
        }

        private static void ClientWorkThread(object clientObject)
        {
            Socket clientSocket = clientObject as Socket;
            while (true)
            {
                try
                {
                    int headerInt = 2;
                    byte[] headerBuffer = new byte[headerInt];

                    int RecvLength = clientSocket.Receive(headerBuffer, headerInt, SocketFlags.None);

                    if (RecvLength > 0)
                    {
                        ushort packetLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));

                        byte[] recvBuffer = new byte[packetLength];
                        RecvLength = clientSocket.Receive(recvBuffer, packetLength, SocketFlags.None);

                        string recvString = Encoding.UTF8.GetString(recvBuffer);

                        string sendString = $"Message : {recvString}";

                        byte[] sendBuffer = MessageToBytes(sendString);

                        lock (lock_ClientSockets)
                        {
                            foreach (Socket sendSocket in clientSockets)
                            {
                                int SendLength = sendSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                            }
                        }
                    }
                    else
                    {
                        // 아직 클라이언트가 정상적으로 나갔을 때는 처리하지 않아서, 이 구문이 작동하는 경우는 없고, 강제종료시 catch에서 처리됨
                        string sendString = $"DisConnected : {clientSocket.RemoteEndPoint}";
                        byte[] sendBuffer = MessageToBytes(sendString);

                        clientSocket.Close();
                        lock (lock_ClientSockets)
                        {
                            clientSockets.Remove(clientSocket);
                            foreach (Socket sendSocket in clientSockets)
                            {
                                int SendLength = sendSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                            }
                        }

                        return;
                    }
                }
                catch (Exception e)
                {
                    string sendMessage = $"에러 발생 클라이언트 : {clientSocket.RemoteEndPoint} // {e.Message}";

                    byte[] sendBuffer = MessageToBytes(sendMessage);

                    clientSocket.Close();
                    lock (lock_ClientSockets)
                    {
                        clientSockets.Remove(clientSocket);
                        foreach (Socket sendSocket in clientSockets)
                        {
                            clientSockets.Remove(clientSocket);
                            int SendLength = sendSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                        }
                    }

                    return;
                }
            }
      
        }

        private static byte[] MessageToBytes(string sendString)
        {
            byte[] dataBuffer = Encoding.UTF8.GetBytes(sendString);
            ushort packetLength = (ushort)dataBuffer.Length;

            int headerInt = 2;
            byte[] headerBuffer = new byte[headerInt];
            headerBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packetLength));

            byte[] sendBuffer = new byte[headerInt + packetLength];
            Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
            Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);

            return sendBuffer;
        }
        
    }
}
