using System.ComponentModel;
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
            byte[] headerBuffer = new byte[2];
            ushort packetLength;


            int RecvLength = clientSocket.Receive(headerBuffer, headerInt, SocketFlags.None);
            packetLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));
            #region 공부정리
            // ○ 엔디언 변환 문제 [정리 내용은 지피티 내용을 참조했기 때문에, 틀린 내용이 있을 수 있음]
            // - 엔디언 변환 : 정수 데이터 <->  바이트 데이터로 변환할 때, 변환하는 방법이 다름(리틀 엔디언 vs 빅 엔디언)
            // ※ 리틀엔디언 : 바이트 배열변환시, 작은단위 순서로 저장. 빅엔디언 : 바이트 배열변환시, 큰단위 순서로 저장. (리틀엔디언을 뒤집으면 빅엔디언과 동일 결과)
            //  -> PC기본값은 리틀 엔디언, 네트워크 표준은 빅 엔디언. 클라이언트와 서버가 다른 플랫폼을 사용시, 엔디언 변환 문제가 발생할 수 있음.
            // 
            // ○ 
            // 엔디언 변환은 바이트 데이터 <-> 정수  를 변환할 때 발생. 따라서 Encoding.UTF8.GetSTring, GetBytes 는 무관. (이미 UTF8 바이트로 인코딩된 상태)
            // 따라서 정수 값이 있는 HeaderBuffer를 변환할 때 엔디언 문제 처리 필요


            // ○ 엔디언 변환 처리에서, 단위변환 문제
            //  - packetLength 의 단위는 ushort, IPAddress.Net or Host.. 의 매개변수 단위는 short
            //  - ushort를 short로 변환시, 랜덤이라 가정할 때, 절반의 확률로 우리가 보는 10진수값이 변함 
            //    Ex) uShort와 short를 16바이트가 아닌, short : [-10, 10] , uShort[0, 20] 이라 가정할 때,
            //        (short) -10 -> (ushort) 10
            //        (short) -11 -> (ishort) 11 ... 로 short가 음수시, ushort변환 시 값이 바뀜. ( 이를 이해하려면 2의 보수 개념을 알아야 하는데 몰라서.. 이렇게로만 이해함)
            //  - 따라서, 엔디언 변환 처리시, 매개변수 값에 BitConvereter.ToUInt16 이 아닌, BitConverter.ToInt16 사용 및 (short)packetLength 로, ushort를 short로 형변환 필요

            #endregion

            byte[] recvBuffer = new byte[packetLength];
            clientSocket.Receive(recvBuffer, packetLength, SocketFlags.None);

            string receiveString = Encoding.UTF8.GetString(recvBuffer);
            Console.WriteLine(receiveString);





            string sendString = "서버에서 클라이언트로 회신하는 메세지입니다";
            byte[] dataBuffer = Encoding.UTF8.GetBytes(sendString);

            packetLength = (ushort)dataBuffer.Length;
            headerBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packetLength));


            byte[] sendBuffer = new byte[headerInt + packetLength];
            Buffer.BlockCopy(headerBuffer, 0, sendBuffer, 0, headerInt);
            Buffer.BlockCopy(dataBuffer, 0, sendBuffer, headerInt, packetLength);

            int SendLength = clientSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);




            serverSocket.Close();
            clientSocket.Close();
        }
    }

}