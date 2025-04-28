using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

class Client
{
    static string serverIP = "127.0.0.1";
    static int port = 5000;
    static NetworkStream? stream = null;

    static void sendMessage(string message, int buffsize = 1024)
    {
        if (stream == null)
            return;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        stream.Write(buffer, 0, buffsize);
    }
    static string GetMessage(int buffsize = 1024)
    {
        if (stream == null)
            return "";
        byte[] buffer = new byte[buffsize];
        stream.Read(buffer, 0, buffsize);
        string ret = Encoding.UTF8.GetString(buffer);

        while (true)
        {
            if (ret[ret.Length - 1] == ' ')
            {
                ret = ret.Remove(ret.Length - 1, 1);
            }
            else
            {
                break;
            }
        }
        return ret;

    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.Write("Type your name: ");
        string a = Console.ReadLine();


        TcpClient tcpClient = new TcpClient(serverIP, port);
        Console.WriteLine("Succes!");

        stream = tcpClient.GetStream();

        sendMessage(a);
        string number = GetMessage();

        Console.WriteLine($"You are {Convert.ToInt32(number)} client");

        while (true)
        {
            try
            {
                Console.WriteLine(GetMessage());
            }
            catch (Exception ex) { break; }
            Console.WriteLine("Type Enter to exit");
            Console.ReadLine();
        }
    }
}
