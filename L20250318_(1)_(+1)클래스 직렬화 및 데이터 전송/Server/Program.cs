using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Server
{
    internal class Program 
    {
        public class Dog
        {
            public string name { get; set; }
            public int age { get; set; }

            #region 공부정리
            // ○ public_변수 [ public ... ] - public_프로퍼티 [public ... {get; set;}] 과, Json직렬화
            // public_변수 는 필드멤버의 메모리를 직접 변경. public_property [public ... {get; set;} 은, getter, setter [ private int exampleInt; public int ExampleInt{ get{ return exampleInt; } set{ exampleInt = value;}
            // 를 사용한다.
            // C#에서 정의된 Json직렬화는, getter, setter로 변경, 리턴 하는 public_프로퍼티만을 대상으로 직렬화가 되도록 만들어짐
            // 따라서 Json직렬화를 위해서는 public_프로퍼티인 [ {get; set;} ] 이 필요
            #endregion
        }


        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            serverSocket.Bind(serverEndPoint);

            serverSocket.Listen(10);

            Socket clientSocket = serverSocket.Accept();

            byte[] buffer = new byte[1024];
            int RecvLength = clientSocket.Receive(buffer);

            string jsonString = Encoding.UTF8.GetString(buffer, 0, RecvLength);
            #region 공부정리
            // Encoding.UTF8.GetString(buffer); 만 할시, [1024]로 할당된 buffer 에 50의 데이터만 들어왔다면, 1024-50의 데이터 또한 빈 데이터로 읽어 0/0/0/0/0/0/0/0/0... 이 입력된다.
            // 클래스 데이터의 경우, 위 빈 데이터 또한 읽기 때문에, 오류가 발생한다.
            // 따라서 모든 데이터가 그런 것은 아니지만, 클래스 데이터를 읽어들일 경우, GetSTring(buffer, 0, RecvLength); 로 읽어 들여야, 빈 데이터를 읽지 않게 해야 오류가 발생하지 않는다.
            // 위 GetSetting(buffer, 0, RecvLength)는 buffer 데이터를, 0(시작지점)부터 RecvLength(길이) 만큼의 데이터만 읽어 변환한다.
            #endregion

            Dog dog = JsonSerializer.Deserialize<Dog>(jsonString);

            Console.WriteLine($"{dog.name} / {dog.age}");

            serverSocket.Close();
            clientSocket.Close();
        }
    }
}

