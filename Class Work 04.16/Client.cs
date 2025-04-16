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

        TcpClient tcpClient = new TcpClient(serverIP, port);
        Console.WriteLine("Succes!");
        Console.ReadLine();
    }
}
