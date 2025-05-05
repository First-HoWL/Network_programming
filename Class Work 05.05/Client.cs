using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System;
class Message
{
    public string user { get; set; }
    public string text { get; set; }
    //public ConsoleColor color { get; set; }
}

class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Charset{ get; set;}

    public Player(int x, int y, char charset = '○')
    {
        X = x;
        Y = y;
        this.Charset = charset; 
    }
    public Player() : this(0, 0, '○') { }

}

class Field
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public ConsoleColor BackgroundColor;
    public ConsoleColor ForegroundColor;

    public Field()
    {
        BackgroundColor = ConsoleColor.Green;
        ForegroundColor = ConsoleColor.Black;
        (X, Y) = (0, 0);
        (Width, Height) = (20, 20);

    }
    public void DrawBackground()
    {
        for (int i = Y; i < Y + Height; i++)
        {
            for (int j = X; j < X + Width + 1; j++)
            {
                Console.SetCursorPosition(j, i);
                Console.BackgroundColor = BackgroundColor;
                Console.Write(" ");
                Console.ResetColor();
            }
        }
        
    }

    public void Draw()
    {
        for (int y = 0; y < Height; y++)
        {
            Console.SetCursorPosition(X, Y + y);
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            for (int x = 0; x < Width; x++)
            {
                
                Player? p = GetPlayerByPosition(x, y);
                if (p == null) Console.Write("  ");
                else Console.Write("[]");
                
            }
            Console.ResetColor();
        }
    }
    public Player? GetPlayerByPosition(int x, int y)
    {
        foreach(Player player in Players)
            if (player.X == x && player.Y == y)
                return player;
        return null;
    }
}




class UDPClientApp
{

    static string serverIP = "127.0.0.1";
    static int port = 5037;
    static IPEndPoint ServerEP = new IPEndPoint(IPAddress.Parse(serverIP), port);
    static UdpClient client = new UdpClient(ServerEP);
    static Player player = new Player();
    static Field field = new Field();
    static readonly object lockObj = new object();
    static void ReadingFromServer()
    {
        //client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
        while (true)
        {
            try
            {
                Player[] Players =
                    JsonSerializer.Deserialize<Player[]>(
                        Encoding.UTF8.GetString(client.Receive(ref ServerEP)).Split(char.MinValue).First()
                        );
                
                lock (lockObj) {
                    field.Players = Players.ToList();
                    field.Players.Add(player);
                };
                
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }



    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;


        
        
        lock (lockObj)
        {
            field.Players.Add(player);
        }
        field.DrawBackground();
        Thread thread = new Thread(ReadingFromServer);
        thread.Start();
        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow: player.X -= 1; break;
                    case ConsoleKey.RightArrow: player.X += 1; break;
                    case ConsoleKey.UpArrow: player.Y -= 1; break;
                    case ConsoleKey.DownArrow: player.Y += 1; break;
                }
                byte[] data1 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));
                client.Send(data1, ServerEP);
            }
            lock (lockObj)
            {
                field.Draw();
            }
        }


        
        //Stopwatch sw = null;
        //Console.WriteLine("Your name:");
        //string name = Console.ReadLine();
        //byte[] data = Encoding.UTF8.GetBytes(name);
        //client.Send(data, ServerEP);

        //Thread thread = new Thread(ReadingFromServer);
        //thread.Start();
        //while (true)
        //{
        //    string text = Console.ReadLine();
        //    if (text != null)
        //    {
        //        byte[] data1 = Encoding.UTF8.GetBytes(text);
        //        client.Send(data1, ServerEP);
        //    }
        //}

        //Console.ReadLine();
    }


}
