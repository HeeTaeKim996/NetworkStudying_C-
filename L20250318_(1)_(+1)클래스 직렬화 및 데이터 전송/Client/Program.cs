using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace Client
{
    internal class Program
    {
        public class Dog
        {
            public string name { get; set; }
            public int age { get; set; }
        }


        static void Main(string[] argS)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            clientSocket.Connect(serverEndPoint);

            Dog dog = new Dog { name = "Harry", age = 14 };

            string jsonString = JsonSerializer.Serialize<Dog>(dog);

            byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
            clientSocket.Send(buffer);

            clientSocket.Close();
        }

    }

}