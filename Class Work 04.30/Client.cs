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
class UDPClientApp
{
    
    static string serverIP = "127.0.0.1";
    static int port = 5037;
    static IPEndPoint ServerEP;
    static UdpClient client;
    static void ReadingFromServer()
    {
        
        while (true)
        {
            try
            {
                //Message? clientMessage =
                //    JsonSerializer.Deserialize<Message>(
                //        Encoding.UTF8.GetString(client.Receive(ref ServerEP)).Split(char.MinValue).First()
                //        );
                string text = Encoding.UTF8.GetString(client.Receive(ref ServerEP));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{text}");
                Console.ResetColor();
            }
            catch (Exception ex) { break; }
        }
    }



    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        client = new UdpClient();
        ServerEP = new IPEndPoint(IPAddress.Parse(serverIP), port);
        Stopwatch sw = null;
        Console.WriteLine("Your name:");
        string name = Console.ReadLine();
        byte[] data = Encoding.UTF8.GetBytes(name);
        //if (message == "ping")
        //    sw = Stopwatch.StartNew();
        client.Send(data, ServerEP);
        //Console.WriteLine("Повідомлення відправленно!");

        //byte[] responce = client.Receive(ref ServerEP);
        //string answer = Encoding.UTF8.GetString(responce).Split(char.MinValue).First();

        //if (answer == "pong")
        //{
        //    sw.Stop();
        //    Console.WriteLine(sw.Elapsed);
        //}
        //else { 
        //    Console.WriteLine($"Отримано відповідь: {answer}");

        //}

        Thread thread = new Thread(ReadingFromServer);
        thread.Start();
        while (true)
        {
            string text = Console.ReadLine();
            if (text != null) { 
                byte[] data1 = Encoding.UTF8.GetBytes(text);
                client.Send(data1, ServerEP);
            }
        }

        Console.ReadLine();
    }


}
