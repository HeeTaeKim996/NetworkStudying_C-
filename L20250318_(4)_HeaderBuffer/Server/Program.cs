using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            serverSocket.Bind(serverEndPoint);
            serverSocket.Listen(10);

            Socket clientSocket = serverSocket.Accept();



            int headerInt = 2;
            byte[] headerBuffer = new byte[headerInt];


            int RecvLength = clientSocket.Receive(headerBuffer, headerInt, SocketFlags.None);
            ushort packetLength = BitConverter.ToUInt16(headerBuffer);

            byte[] recvBuffer = new byte[packetLength];
            RecvLength = clientSocket.Receive(recvBuffer, packetLength, SocketFlags.None);

            string receiveString = Encoding.UTF8.GetString(recvBuffer);
            Console.WriteLine(receiveString);




            string sendString = "서버에서 클라이언트로 회신하는 메세지입니다";
            byte[] dataBuffer = Encoding.UTF8.GetBytes(sendString);

            packetLength = (ushort)dataBuffer.Length;
            headerBuffer = BitConverter.GetBytes(packetLength);

            byte[] sendBuffer = new byte[headerInt + packetLength];
            Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
            Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);
            #region 공부정리
            // ○ Buffer.BlockCopy
            // Buffer.BlockCopy : 비트배열을 복사해서 붙여넣는 C#내장함수
            // (1) : 복사되는 비트배열 , (2) : 복사되는 비트배열의 시작지점, (3) : 복사받는 비트배열, (4) : 복사받는 비트배열의 시작지점, (5) : 복사되는 길이
            #endregion

            int SendLength = clientSocket.Send(sendBuffer);


            serverSocket.Close();
            clientSocket.Close();
        }
    }
}