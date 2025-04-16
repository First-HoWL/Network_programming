using System.Text;
using System.Net;
using System.Net.Sockets;

class Client
{
    static string serverIP = "127.0.0.1";
    static int port = 5000;

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.Write("Type your name: ");
        string a = Console.ReadLine();


        TcpClient tcpClient = new TcpClient(serverIP, port);
        Console.WriteLine("Succes!");



        byte[] buffer = Encoding.UTF8.GetBytes(a);
        NetworkStream stream = tcpClient.GetStream();
        stream.Write(buffer, 0, buffer.Length);

        NetworkStream stream2 = tcpClient.GetStream();
        byte[] buffer2 = new byte[1024];
        stream2.Read(buffer2, 0, buffer2.Length);

        string number = Encoding.UTF8.GetString(buffer2);
        Console.WriteLine($"You are {Convert.ToInt32(number)} client");

        Console.ReadLine();
    }
}
