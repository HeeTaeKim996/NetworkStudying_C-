using System.Net;
using System.Net.Sockets;


namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            listenSocket.Bind(listenEndPoint);
            listenSocket.Listen(10);

            Socket clientSocket = listenSocket.Accept();

            using (FileStream fsInput = new FileStream("1.webp", FileMode.Open))
            #region 공부정리
            // ○ FileStream 
            // FileStream 은 C# 내장 클래스로 모든 유형의 확장자를 Binary 데이터로 읽고, 쓸 수 있게 한다.
            #endregion
            {
                byte[] buffer = new byte[400];

                int readSize = 0;

                do
                {
                    readSize = fsInput.Read(buffer, 0, buffer.Length);
                    int SendSize = clientSocket.Send(buffer, readSize, SocketFlags.None);

                    Console.WriteLine($"Server_ReadSize : {readSize}");
                } while (readSize > 0);
                #region 공부정리
                // ○ 보내려는 데이터가 buffer 크기보다 클 때
                // 보내려는 데이터가 buffer 크기보다 클 때, 위처럼 하면 된다.
                // fsInput.Read로 보낼수 있는 buffer의 최댓값만큼을 (int)readSize로 변환한다. fsInput.Read로 모든 데이터를 보냈다면, readSize = 0이되어 do-While이 종료된다. 
                // 따라서 readSize = 0 이 될 때까지, buffer를 readSize만큼 분할하여 데이터를 전송한다.
                #endregion

                fsInput.Close();
            }

            listenSocket.Close();
            clientSocket.Close();
        }
    }
}
