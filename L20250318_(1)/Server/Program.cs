using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, 4000);
            listenSocket.Bind(listenEndPoint);

            listenSocket.Listen(10);
            #region 공부정리
            //listen(n) : 최대 n개의 클라이언트 소켓이 대기. listen은 n개 이상의 소켓이 가능하지만, 클라이언트가 Connect를 할 시, 서버는 Accept하여 처리하는데, 이 때 Accept하기 까지 대기열이 최대 n개 라는 것
            // 만약 10개 초과의 클라이언트가 Connect하고, 서버가 Accept처리를 할 때 대기가 n개를 초과하면, 초과한 상태의 클라이언트는 삭제
            #endregion
            Socket clientSocket = listenSocket.Accept();


            byte[] buffer = new byte[1024];
            int RecvLength = clientSocket.Receive(buffer);

            string jsonString = Encoding.UTF8.GetString(buffer);

            JObject json = JObject.Parse(jsonString);
            #region 공부정리
            // 클라이언트간 데이터를 주고 받기 위해서는 직렬화된 binary 데이터를 사용.
            // binary 데이터를 변환하기 위해서 다양한 방법이 있는데, Key-Value로 구성된 JsonData가 자주 사용됨
            // 클래스의 인스턴스 또한 필드 멤버들이 변수명 - 변수값으로 저장되기 때문에, JsonData로 자주 변환되어 사용함. ※ L250318_(1)_(+1)클래스 직렬화 및 데이터 전송 참조
            // NetwonSoft.Json.Linq (nuget) 은 위 클래스 Json변환 작업을 단순화하여 처리하기 위해,  JObject라는 클래스를 사용. JObject는 key - value 로 데이터를 저장.
            #endregion
            if (json.Value<string>("message").ToString().CompareTo("안녕하세요") == 0)
            {
                byte[] message;
                JObject result = new JObject();
                result.Add("message", "반가워");
                message = Encoding.UTF8.GetBytes(result.ToString());
                int SendLength = clientSocket.Send(message);
            }

            clientSocket.Close();
            listenSocket.Close();
        }
    }

}
