using System.Text;
using System.Net;
using System.Net.Sockets;


class Server
{
    static TcpListener listener; 
    static int port = 5000;
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine("Server Started!");

        while (true) { 
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }



    }
    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;

        Console.WriteLine("New Client");
        Console.WriteLine(client.Client.RemoteEndPoint.ToString()); 
    }

}
