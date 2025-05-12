// CLIENT
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;
class Message
{
    public string user { get; set; }
    public string text { get; set; }
    //public ConsoleColor color { get; set; }
}
class Client
{
    static string serverIP = "127.0.0.1";
    static int port = 5000;
    static NetworkStream? stream = null;
    static ConsoleColor Color;
    static void sendMessage(string message, int buffsize = 1024)
    {
        if (stream == null)
            return;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }
    static string GetMessage(int buffsize = 1024)
    {
        if (stream == null)
            return "";
        byte[] buffer = new byte[buffsize];
        stream.Read(buffer, 0, buffsize);
        string ret = Encoding.UTF8.GetString(buffer).Split(char.MinValue).First();

        return ret;

    }

    static void ReadingFromServer()
    {
        while (true)
        {
            try
            {
                var a = GetMessage();
                //Console.WriteLine(a);
                Message? clientMessage =
                    JsonSerializer.Deserialize<Message>(a);
                //Console.ForegroundColor = clientMessage.color;
                if (clientMessage.user.ToLower() == "server")
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{clientMessage.user}: {clientMessage.text}");
                Console.ResetColor();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                break; }
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.Write("Type your name: ");
        string name = Console.ReadLine();


        TcpClient tcpClient = new TcpClient(serverIP, port);
        Console.WriteLine("Succes!");

        stream = tcpClient.GetStream();

        sendMessage(name);
        
        Thread serverOutputThread = new Thread(ReadingFromServer);
        serverOutputThread.Start();

        while (true)
        {
            string text = Console.ReadLine();
            sendMessage(
                JsonSerializer.Serialize(
                    new Message { user = name, text = text }));
        }

        Console.WriteLine("Type Enter to exit");
        Console.ReadLine();
    }
}
