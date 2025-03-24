using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            listenSocket.Bind(serverEndPoint);
            listenSocket.Listen(10);


            List<Socket> clientSockets = new List<Socket>();
            List<Socket> checkRead = new List<Socket>();

            while (true)
            {
                checkRead.Clear();
                checkRead = new List<Socket>(clientSockets);
                checkRead.Add(listenSocket);

                Socket.Select(checkRead, null, null, -1);
                #region 공부정리
                // (1)항 : List<Socket>을 입력. 해당 리스트에서 읽기 가능한 소켓을 감지
                // (2)항 : null 시 쓰기 가능한 소켓은 감지하지 않음
                // (3)항 : null 시 오류 발생한 소켓은 감지하지 않음
                // (4)항 : -1시 무한대기. 0초과시 n밀리초 단위로 대기 [ 단위가 초 vs 밀리초긴 하지만, 유니티의 yield return new WaitForSeconds(n); 과 유사 ]
                #endregion


                foreach (Socket findSocket in checkRead)
                {
                    if(findSocket == listenSocket)
                    {
                        Socket clientSocket = listenSocket.Accept();
                        clientSockets.Add(clientSocket);
                        Console.WriteLine($"Client Connected : {clientSocket.RemoteEndPoint}");
                    }
                    else
                    {
                        try
                        {
                            int headerInt = 2;
                            byte[] headerBuffer = new byte[headerInt];
                            int RecvLength = findSocket.Receive(headerBuffer, headerInt, SocketFlags.None);

                            if(RecvLength > 0)
                            {
                                ushort packetLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));

                                byte[] recvBuffer = new byte[packetLength];
                                RecvLength = findSocket.Receive(recvBuffer, packetLength, SocketFlags.None);
                                string recvString = Encoding.UTF8.GetString(recvBuffer);




                                string castString = "Message : " + recvString;
                                byte[] dataBuffer = Encoding.UTF8.GetBytes(castString);
                                packetLength = (ushort)dataBuffer.Length;
                                headerBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packetLength));

                                byte[] sendBuffer = new byte[headerInt + packetLength];
                                Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
                                Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);

                                foreach(Socket sendSocket in clientSockets)
                                {
                                    int SendLength = sendSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                                }
                            }
                            else
                            {
                                string castString = $"Disconnected : {findSocket.RemoteEndPoint}";

                                byte[] dataBuffer = Encoding.UTF8.GetBytes(castString);
                                ushort packetLength = (ushort)dataBuffer.Length;
                                headerBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packetLength));

                                byte[] sendBuffer = new byte[headerInt + packetLength];
                                Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
                                Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);


                                findSocket.Close();
                                clientSockets.Remove(findSocket);


                                foreach (Socket sendSocket in clientSockets)
                                {
                                    int SendLength = sendSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                                }

                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"에러발생 클라이언트 : {findSocket.RemoteEndPoint} // {e.Message}");

                            string castString = $"Disconnected : {findSocket.RemoteEndPoint}";

                            byte[] dataBuffer = Encoding.UTF8.GetBytes(castString);
                            ushort packetLength = (ushort)dataBuffer.Length;

                            byte[] headerBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packetLength));
                            int headerInt = 2;

                            byte[] sendBuffer = new byte[headerInt + packetLength];
                            Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
                            Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);


                            findSocket.Close();
                            clientSockets.Remove(findSocket);


                            foreach (Socket sendSocket in clientSockets)
                            {
                                int SendLength = sendSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                            }
                        }
                    }
                }

                                           

                //Server 작업
                {
                    Console.WriteLine("서버작업");
                }
            }


            listenSocket.Close();
        }

    }
}