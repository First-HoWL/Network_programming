
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Text.Json;
using System.Drawing;

class Message
{
    public string user { get; set; }
    public string text { get; set; }
    public ConsoleColor color { get; set; }
}
class Server
{
    static TcpListener listener;
    static int port = 5000;
    static int clients = 1;
    static readonly object lockObj = new object();
    static Random rand = new Random();
    //static ConsoleColor Color;

    static List<ConsoleColor> colors = new List<ConsoleColor>
        {
            ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.DarkGreen, ConsoleColor.Magenta, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Yellow,
        };

    static void sendMessage(NetworkStream stream, string message, int buffsize = 1024)
    {
        if (stream == null)
            return;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }
    static string GetMessage(NetworkStream stream, int buffsize = 1024)
    {
        if (stream == null)
            return "";
        byte[] buffer = new byte[buffsize];
        stream.Read(buffer, 0, buffsize);
        string ret = Encoding.UTF8.GetString(buffer).Split(char.MinValue).First();

        return ret;
    }
    

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;


        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine("Server Started!");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    static List<TcpClient> Clients = new List<TcpClient>();
    static void Broadcast(Message message)
    {
        Console.Write($"{message.user}: ");
        Console.ForegroundColor = message.color;
        Console.WriteLine(message.text);
        Console.ResetColor();
        foreach (var item in Clients.ToArray())
        {
            try { 
                sendMessage(item.GetStream(), JsonSerializer.Serialize(message));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    static void Broadcast(string message)
    {
        Console.WriteLine($"Broadcast: {message}");
        foreach (var item in Clients.ToArray())
        {
            sendMessage(item.GetStream(), message);
        }
    }
    static void Broadcast(string message, TcpClient client)
    {
        Console.WriteLine($"Broadcast: {message}");
        foreach (var item in Clients.ToArray())
        {
            if (item != client)
                sendMessage(item.GetStream(), message);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        Console.WriteLine();
        Console.WriteLine("New Client");
        ConsoleColor Color = colors[rand.Next(0, colors.Count())];
        var endPoint = client.Client.RemoteEndPoint.ToString();
        Clients.Add(client);
        var stream = client.GetStream();
        string name = GetMessage(stream);
        Console.WriteLine($"Name {name} | {endPoint.ToString()}");


        lock (lockObj)
        {
            sendMessage(stream, Convert.ToString(clients++));
        }
        Broadcast(new Message{ user = "SERVER", text = $"{name} ({endPoint}) Connect to Server", color = Color });
        try
        {
            while (true)
            {
                Message? clientMessage = 
                    JsonSerializer.Deserialize<Message>(GetMessage(stream));
                if (clientMessage != null)
                    Broadcast(new Message {user = clientMessage.user, text = clientMessage.text, color = Color });
            }
           
        }
        catch (Exception ex)
        {
            Broadcast($"{name} ({endPoint}) Disconected!", client);
        }
        finally
        {
            lock (client)
                Clients.Remove(client);

            client.Close();
        }


    }

}
