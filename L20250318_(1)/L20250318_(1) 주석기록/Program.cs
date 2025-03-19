using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            #region 공부기록
            // ○ AddresFamily 
            // AddressFamily.Internetwork : Ipv4(32비트체계) 주소체계 사용.
            // ※ AddresFamily.InternetworkV6 : IPv6(128비트체계) 주소체계. 

            // ○ SocketType - ProtocolType
            // ProtocopType.Tcp -> SocketType.Stream 사용. ProtocolType.Udp -> SocketType.Dgram 사용
            // - SocketType.Stream : 송수신 순서 보장. 송신 실패 확인시 재발송. 위 기능들로 데이터 지연 가능성 있음.
            // - SocketType.Dgram : 순서 보장 없음. 송신 실패 확인을 안하기 때문에, 데이터 손실 가능성 있음
            // -> client간 지연없는 빠른 반응이 필요할 때(FPS의 탄알 피격 효과 등)에 Udp, 데이터 누락이 없어야 할 시 Tcp 사용
            #endregion
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
            clientSocket.Connect(listenEndPoint);
            #region 공부기록
            // ○ IPAdress : IP번호. 127.0.0.1 은 loopBack으로 자기 자신의 주소를 의미

            // ○ Port (2항) : 네트워크 서비스 식별번호. HTTP(80), HTTPS(443)..
            #endregion
            string jsonString = "{\"message\" : \"안녕하세요\"}";
            #region 공부기록
            // 위의 우항은 Json형식을 보여준 것일 뿐, Json String 역할을 하지 못한다 함(Fro지피티). 실제로 단순히 string st = "안녕하세요"; 하고 Encoding.UTF8.GetBytes(st); 를 해도 잘 작동함
            #endregion
            byte[] message = Encoding.UTF8.GetBytes(jsonString);
            #region 공부정리
            // UTF8 : UTF-xx는 문자열의 바이트 차이. UTF8은 가변길이(1~4바이트). Json, 네트워크통신, 웹표준 등에서 UTF8 사용
            #endregion
            int sendLength = clientSocket.Send(message);


            byte[] buffer = new byte[1024];
            int RecvLength = clientSocket.Receive(buffer);
            string JsonString = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(JsonString);

            clientSocket.Close();
        }
    }
}
