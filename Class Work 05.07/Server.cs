
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

    public Player(int x, int y, char charset = '○')
    {
        X = x;
        Y = y;
        this.Charset = charset;
    }
    public Player() : this(0, 0, '○') { }

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
    static int port = 5037;
    static Dictionary<IPEndPoint, Player> Players =
        new Dictionary<IPEndPoint, Player>();

    static List<IPEndPoint> points = new List<IPEndPoint>();
    static List<User> Users = new List<User>();


    static Player[] GetResponseForClient(IPEndPoint client)
    {
        List<Player> response = new List<Player>();
        foreach (var player in Players)
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
        foreach (var b in points)
        {
            Player[] response = GetResponseForClient(b);
            var a = JsonSerializer.Serialize(response);
            Console.WriteLine(a);
            byte[] data1 = Encoding.UTF8.GetBytes(a);
            Server1.Send(data1, b);
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

    
    static UdpClient Server1 = new UdpClient(port);
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        
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
                    Players.Add(remoteEP, player);
                    points.Add(remoteEP);
                    Console.WriteLine($"New Connection({remoteEP})!");
                }
                else
                {
                    Players[remoteEP] = player;
                }
                //Player[] response = new Player[Players.Count - 1];
                //foreach (Player pl in Players.Values)
                //    if (player != pl) response.Append(pl);

                
                Broadcast();

            }


        }


    }

}
