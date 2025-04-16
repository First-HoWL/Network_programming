using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;


class Server
{
    static TcpListener listener; 
    static int port = 5000;
    static int clients = 1;
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
        Console.WriteLine();
        Console.WriteLine("New Client");

        var endPoint = client.Client.RemoteEndPoint.ToString();
        

        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
        
        string name = Encoding.UTF8.GetString(buffer);
        Console.WriteLine($"Name {name} | {endPoint.ToString()}");
        byte[] buffer2 = Encoding.UTF8.GetBytes(Convert.ToString(clients++));
        stream.Write(buffer2, 0, buffer2.Length);
    }

}
