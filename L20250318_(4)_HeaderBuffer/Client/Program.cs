using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            clientSocket.Connect(serverEndPoint);


            int headerInt = 2;
            byte[] headerBuffer = new byte[headerInt];

            string sendString = "클라이언트에서 서버로 보내는 메세지입니다";
            byte[] dataBuffer = Encoding.UTF8.GetBytes(sendString);
            ushort packetLength = (ushort)dataBuffer.Length;
            headerBuffer = BitConverter.GetBytes(packetLength);

            byte[] sendBuffer = new byte[headerInt + packetLength];
            Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
            Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);

            int SendLength = clientSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);




            int RecvLength = clientSocket.Receive(headerBuffer, headerInt, SocketFlags.None);
            packetLength = BitConverter.ToUInt16(headerBuffer);

            byte[] recvBuffer = new byte[packetLength];
            RecvLength = clientSocket.Receive(recvBuffer, packetLength, SocketFlags.None);

            string receiveString = Encoding.UTF8.GetString(recvBuffer);
            Console.WriteLine(receiveString);



            clientSocket.Close();
        }
    }
}