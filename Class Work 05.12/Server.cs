// SERVER

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Text.Json;
using System.Drawing;
using System.Reflection;

class Message
{
    public string user { get; set; }
    public string text { get; set; }
    //public ConsoleColor color { get; set; }
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

        Thread thread123 = new Thread(checkTime);
        thread123.Start();

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    static List<TcpClient> Clients = new List<TcpClient>();
    static Dictionary<TcpClient, string> ClientsName = new Dictionary<TcpClient, string>();
    static Dictionary<TcpClient, DateTime> ClientsTime = new Dictionary<TcpClient, DateTime>();
    static List<TcpClient> ClientsINGame = new List<TcpClient>();
    static void Broadcast(Message message)
    {
        Console.Write($"{message.user}: ");
        //Console.ForegroundColor = message.color;
        Console.WriteLine(message.text);
        //Console.ResetColor();
        foreach (var item in Clients.ToArray())
        {
            try
            {
                sendMessage(item.GetStream(), JsonSerializer.Serialize(message));
            }
            catch (Exception ex)
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
            if (!item.Equals(client))
                sendMessage(item.GetStream(), message);
        }
    }

    static string PlayerName(TcpClient client)
    {
        if (client == null)
            return "";
        foreach (var player in ClientsName)
            {
                if (player.Key.Equals(client))
                    return player.Value;
            }
        return "";
    }

    static void checkTime()
    {
        while (true)
        {
            Thread.Sleep(1000);
            if (GameStarted)
                foreach (var PDateTime in ClientsTime)
                {
                    if (DateTime.Now.Second - PDateTime.Value.Second >= 8)
                    {
                        foreach (var PDateTime2 in ClientsTime)
                        {
                            ClientsTime[PDateTime2.Key] = DateTime.Now;
                        }
                        CurrentPlayer = ClientsINGame[ClientsINGame.IndexOf(CurrentPlayer) == ClientsINGame.Count - 1 ? 0 : ClientsINGame.IndexOf(CurrentPlayer) + 1];
                        Broadcast(new Message { user = "Server", text = PlayerName(CurrentPlayer) + "\'s turn" });

                    }
                }
        }
    }


    static int readyPlayers;
    static bool GameStarted = false;
    static int number;
    static TcpClient CurrentPlayer = new TcpClient();
    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        Console.WriteLine();
        Console.WriteLine("New Client");
        //ConsoleColor Color = colors[rand.Next(0, colors.Count())];
        var endPoint = client.Client.RemoteEndPoint.ToString();
        Clients.Add(client);
        var stream = client.GetStream();
        string name = GetMessage(stream);
        Console.WriteLine($"Name {name} | {endPoint.ToString()}");
        ClientsName.Add(client, name);
        ClientsTime.Add(client, DateTime.Now);
        bool isReady = false;

        Broadcast(new Message { user = "SERVER", text = $"{name} connect to Server" });
        try
        {
            while (true)
            {
                Message? clientMessage =
                    JsonSerializer.Deserialize<Message>(GetMessage(stream));
                ClientsTime[client] = DateTime.Now;
                if (GameStarted) { 
                    isReady = false;
                    readyPlayers = 0;
                }

                if (GameStarted)
                {
                    if (ClientsINGame.Contains(client))
                        if (client != CurrentPlayer)
                        {
                            sendMessage(stream, JsonSerializer.Serialize(new Message { user = "Server", text = $"You are not the current player!" }));
                        }
                        else
                        {
                            int? numb = null;
                            try
                            {
                                numb = Convert.ToInt32(clientMessage.text);
                                Broadcast(new Message { user = clientMessage.user, text = clientMessage.text });
                                if (numb == number)
                                {
                                    Broadcast(new Message { user = "Server", text = $"{name} type the correct answer({number})!" });
                                    GameStarted = false;
                                }
                                else { 
                                    if (numb < number)
                                    {
                                        Broadcast(new Message { user = "Server", text = $"Correct number bigger than current one" });
                                    }
                                    else if (numb > number)
                                    {
                                        Broadcast(new Message { user = "Server", text = $"Correct number smaller than current one" });
                                    }
                                    CurrentPlayer = ClientsINGame[ClientsINGame.IndexOf(CurrentPlayer) == ClientsINGame.Count - 1 ? 0 : ClientsINGame.IndexOf(CurrentPlayer) + 1];
                                    Thread.Sleep(200);
                                    Broadcast(new Message { user = "Server", text = PlayerName(CurrentPlayer) + "\'s turn" });
                                }
                            }
                            catch (Exception e)
                            {
                                sendMessage(stream, JsonSerializer.Serialize(new Message { user = "Server", text = $"Not a number" }));
                            }
                        }
                    else
                    {
                        sendMessage(stream, JsonSerializer.Serialize(new Message { user = "Server", text = $"The game was started, wait a minute" }));
                    }


                }

                else if (clientMessage.text.ToLower() == "ready" && isReady != true) {
                    isReady = true;
                    Broadcast(new Message { user = "Server", text = $"{name} ready({++readyPlayers}/{Clients.Count})" });
                    if (readyPlayers == Clients.Count)
                    {
                        Broadcast(new Message { user = "Server", text = $"Game started!" });
                        GameStarted = true;
                        readyPlayers = 0;
                        number = rand.Next(1, 101);
                        ClientsINGame.Clear();
                        Console.WriteLine(number);
                        foreach (var clt in Clients)
                            ClientsINGame.Add(clt);
                        CurrentPlayer = ClientsINGame[0];
                        Broadcast(new Message { user = "Server", text = PlayerName(CurrentPlayer) + "\'s turn" });
                    } 
                }
                else if (clientMessage != null)
                    Broadcast(new Message { user = clientMessage.user, text = clientMessage.text });
                
            }

        }
        catch (Exception ex)
        {
            Broadcast($"{name} ({endPoint}) Disconected!", client);
        }
        finally
        {
            lock (client) { 
                Clients.Remove(client);
                ClientsINGame.Remove(client);
            }
            client.Close();
        }


    }

}
