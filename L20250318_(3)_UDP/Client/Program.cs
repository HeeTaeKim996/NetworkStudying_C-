using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    internal class Client
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 6000);

            byte[] buffer = new byte[1024];

            string message = "안녕하세요";
            buffer = Encoding.UTF8.GetBytes(message);
            int SendLength = clientSocket.SendTo(buffer, buffer.Length, SocketFlags.None, serverEndPoint);
            #region 공부정리
            // ○ SocketFlags 
            // SocketFlags는 데이터 송수신 때 추가 설정을 하는 클래스. SocketFlags.None은 아무 설정 없은 기본값
            // 예를 들어 SocketFlags.DontRoute 는 라우터를 거치지 않고 직접 패킷 전송
            #endregion


            byte[] buffer2 = new byte[1024];
            EndPoint remoteEndPoint = (EndPoint)serverEndPoint;
            #region 공부정리
            // ○ IPEndPoint 와 EndPoint
            // EndPoint는 추상클래스이고, IPEndPoint는 EndPoint를 상속한, 인스턴스가능한 클래스. 
            // 이 코드에서 Server, Client Project를 비교해보면, EndPoint가 할당되지 않은 처음에는 SendTo 의 (3)항을 IPEndPoint로 사용하고, ReceiveFrom 의 (3)항의 ref 로 EndPoint가 할당되면, 이후 SendTo 에서는 EndPoint를 사용한다.
            // SendTo의 (3)항은 IPEndPoint 와 EndPoint가 둘 다 사용가능하지만(매개변수를 EndPoint로 받기 때문에), EndPoint가 할당된다면 EndPoint를 사용하는 듯하다.
            // 왜냐하면,
            // 1) 처음 할당한 IPEndPoint와, SendTo로 보내야할 곳의 주소가 변경될 수 있기 때문에, 최근에 받은 주소인 EndPoint로 보내는 것이 오류 가능성이 적다
            // 2) SendTo 는 매개변수로 EndPoint를 받기 때문에, IPEndPoint로 보낼시 추가 형변환이 필요하기 때문에 비효율적이다.
            #endregion

            int RecvLength = clientSocket.ReceiveFrom(buffer2, ref remoteEndPoint);
            string message2 = Encoding.UTF8.GetString(buffer2);

            Console.WriteLine($"Client_수신 : {message2}");

            clientSocket.Close();
        }
    }
}
