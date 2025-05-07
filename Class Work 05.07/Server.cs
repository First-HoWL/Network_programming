
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Text.Json;
using System.Drawing;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Charset { get; set; }

    public ConsoleColor color { get; set; }

    public Player(int x, int y, ConsoleColor color, char charset = '○')
    {
        this.color = color;
        this.X = x;
        this.Y = y;
        this.Charset = charset;
    }
    public Player() : this(0, 0, ConsoleColor.Black, '○') { }

}


class Message
{
    public string user { get; set; }
    public string text { get; set; }
}

class User
{
    public string name { get; set; }
    public IPEndPoint IPPoint { get; set; }
}

class Server
{
    static List<ConsoleColor> colors = new List<ConsoleColor>
        {
            ConsoleColor.Red, ConsoleColor.DarkGreen, ConsoleColor.Magenta, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Yellow,
        };
    static int port = 5037;
    static Dictionary<IPEndPoint, Player> Players =
        new Dictionary<IPEndPoint, Player>();

    static Dictionary<IPEndPoint, DateTime> PlayersDateTime =
        new Dictionary<IPEndPoint, DateTime>();

    static List<IPEndPoint> points = new List<IPEndPoint>();
    static List<User> Users = new List<User>();
    static readonly object lockPlayers = new object();
    static readonly object lockPDT = new object();
    static readonly object lockPoints = new object();


    static Player[] GetResponseForClient(IPEndPoint client)
    {
        List<Player> response = new List<Player>();
        Dictionary<IPEndPoint, Player> Players2;
        lock (lockPlayers)
        {
            Players2 = Players;
        }

        foreach (var player in Players2)
        {
            if (!player.Key.Equals(client))
            {
                response.Add(player.Value);
                //Console.WriteLine("ABOBA!");
            }
        }
        return response.ToArray();
    }


    static void Broadcast(UdpClient Server, string message, string name)
    {
        Console.WriteLine($"Broadcast: {name}: {message}");
        byte[] data = Encoding.UTF8.GetBytes($"{name} : {message}");
        foreach (var p in Users)
        {

            Server.Send(data, p.IPPoint);
        }
    }

    static void Broadcast()
    {
        lock (lockPoints) { 
            foreach (var b in points)
            {
                Player[] response = GetResponseForClient(b);
                var a = JsonSerializer.Serialize(response);
                Console.WriteLine(a);
                byte[] data1 = Encoding.UTF8.GetBytes(a);
                Server1.Send(data1, b);
            }
        }
    }

    static bool isNew(IPEndPoint obj)
    {
        foreach (var item in Users)
        {
            if (item.IPPoint.Equals(obj))
            {
                return false;
            }
        }
        return true;
    }

    static string GetName(IPEndPoint remoteEP)
    {
        foreach (var item in Users)
        {
            if (item.IPPoint.Equals(remoteEP))
            {
                return item.name;
            }
        }
        return "";
    }

    static void CheckTime()
    {
        while (true) {
            Thread.Sleep(1000);
            foreach (var PDateTime in PlayersDateTime)
            {
                if (DateTime.Now.Second - PDateTime.Value.Second >= 15)
                {
                    lock (lockPoints)
                        lock (lockPDT)
                            lock (lockPlayers) { 
                                PlayersDateTime.Remove(PDateTime.Key);
                                points.Remove(PDateTime.Key);
                                Players.Remove(PDateTime.Key);
                            }
                    Console.WriteLine($"{PDateTime.Key} Deleted!");
                }
            }
        }
    }


    static Random rand = new Random();
    static UdpClient Server1 = new UdpClient(port);
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;
        // Console.WriteLine(DateTime.Now - DateTime.Now.AddHours(1));
        Thread thread = new Thread(CheckTime);
        thread.Start();
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("Очіквання повідомлень...");
        while (true)
        {
            byte[] data = Server1.Receive(ref remoteEP);
            string text = Encoding.UTF8.GetString(data);
            Player player = JsonSerializer.Deserialize<Player>(text);
            
            if (player != null)
            {

                if (!Players.ContainsKey(remoteEP))
                {
                    player.color = colors[rand.Next(0, colors.Count())];
                    lock (lockPlayers)
                        lock (lockPDT)
                            lock (lockPlayers)
                            {
                                Players.Add(remoteEP, player);
                                PlayersDateTime.Add(remoteEP, DateTime.Now);
                                points.Add(remoteEP);
                            }
                    Console.WriteLine($"New Connection({remoteEP})!");

                    
                    var a = JsonSerializer.Serialize(player);
                    Console.WriteLine(a);
                    byte[] data1 = Encoding.UTF8.GetBytes(a);
                    Server1.Send(data1, remoteEP);

                }
                else
                {
                    lock (lockPlayers)
                        lock (lockPDT) { 
                            Players[remoteEP] = player;
                            PlayersDateTime[remoteEP] = DateTime.Now;
                        }
                }
                

                
                Broadcast();

            }


        }


    }

}
