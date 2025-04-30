
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
    // public ConsoleColor color { get; set; }
}

class User
{
    public string name { get; set; }
    public IPEndPoint IPPoint { get; set; }
}

class Server
{
    static int port = 5037;
    

    static List<User> Users = new List<User>();


    static void Broadcast(UdpClient Server, string message, string name)
    {
        Console.WriteLine($"Broadcast: {name}: {message}");
        byte[] data = Encoding.UTF8.GetBytes($"{name} : {message}");
        foreach (var p in Users)
        {
            
            Server.Send(data, p.IPPoint);
        }
    }
    static bool isNew(IPEndPoint obj)
    {
        foreach(var item in Users)
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

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;
        
        UdpClient Server = new UdpClient(port);
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("Очіквання повідомлень...");
        while (true) { 
            
            byte[] data = Server.Receive(ref remoteEP);
            string text = Encoding.UTF8.GetString(data);
            //Console.WriteLine($"{remoteEP}");
            if (isNew(remoteEP)) { 
                Users.Add(new User { name = text, IPPoint = remoteEP });
                Broadcast(Server, "New Conection", "Server");
            }
            else if (text != "")
                Broadcast(Server, text, GetName(remoteEP));
            
        


            //if (message.ToLower() == "ping")
            //{
            //    Server.Send(Encoding.UTF8.GetBytes("pong"), remoteEP);
            //}
            //else { 
            //    Console.WriteLine($"{remoteEP}: {message}");
           
            //    Server.Send(Encoding.UTF8.GetBytes("Ok!"), remoteEP);
            //    Console.WriteLine("Відповідь відправленно!");
            //}
        }
        

    }

}
