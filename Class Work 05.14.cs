using System.Text;
using System.Threading;
using System;
using System.Text.Json;
using System.Net;
using System.Drawing;

namespace Game
{

    class ConsoleWindow
    {
        public Point from;
        public Point to;
        public ConsoleColor ForegroundColor;
        public ConsoleColor BackgroundColor;
        List<string> text = new List<string>();
        public object lockObj;
        public object lockMessages;
        public bool Boards { get; set; }
        public ConsoleWindow(Point from, Point to, object lockObj, object lockMessages, ConsoleColor ForegroundColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black)
        {
            this.from = from;
            this.to = to;
            this.ForegroundColor = ForegroundColor;
            this.BackgroundColor = BackgroundColor;
            this.lockObj = lockObj;
            this.lockMessages = lockMessages;
        }

        public void DrawBoards()
        {
            lock (lockObj)
            {
                for (int i = from.Y - 1; i < to.Y; i++)
                {
                    Console.SetCursorPosition(from.X - 1, i);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
                for (int i = from.Y - 1; i < to.Y + 0; i++)
                {
                    Console.SetCursorPosition(to.X + 1, i);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
                for (int j = from.X - 1; j < to.X + 1; j++)
                {
                    Console.SetCursorPosition(j, from.Y - 1);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
                for (int j = from.X - 1; j < to.X + 2; j++)
                {
                    Console.SetCursorPosition(j, to.Y);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
            }
        }

        public void Draw()
        {
            lock (lockObj)
                lock (lockMessages)
                {
                    int i = 0;
                    int j = 0;
                    if (Boards == true)
                        DrawBoards();
                    foreach (var messages in text)
                    {
                        Console.BackgroundColor = BackgroundColor;
                        Console.ForegroundColor = ForegroundColor;
                        for (int h = 0; h < messages.Length; h++)
                        {
                            if (i == to.X - from.X)
                            {
                                i = 0;
                                j++;
                            }

                            if (messages[h] == '\n')
                            {
                                i = 0;
                                j++;
                            }
                            else
                            {
                                Console.SetCursorPosition(from.X + i++, from.Y + j);
                                Console.Write(messages[h]);
                            }
                        }
                        Console.ResetColor();
                    }
                }

        }

        public void DrawBackground()
        {
            lock (lockObj)
            {
                for (int i = from.Y; i < to.Y; i++)
                {
                    for (int j = from.X; j < to.X + 1; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.BackgroundColor = BackgroundColor;
                        Console.Write(" ");
                        Console.ResetColor();
                    }
                }
            }
        }

        public void Clear()
        {
            lock (lockObj)
                lock (lockMessages)
                {
                    text.Clear();
                    DrawBackground();
                }
        }

        public void WriteLine(string message)
        {
            lock (lockMessages)
            {
                if (text.Count() >= to.Y - from.Y - 1)
                {
                    text.Remove(text[0]);
                }
                while (message.Length > to.X - from.X)
                {
                    if (text.Count() >= to.Y - from.Y - 1)
                    {
                        text.Remove(text[0]);
                    }
                    string a = "";
                    for (int i = 0; i < to.X - from.X; i++)
                    {
                        a += message[0];
                        message = message.Remove(0, 1);
                    }
                    text.Add(a);
                    a = "";
                }

                text.Add(message + "\n");
            }
        }

        public void Write(string message)
        {
            lock (lockMessages)
            {

                while (message.Length > to.X - from.X)
                {
                    if (text.Count() > to.Y - from.Y)
                    {
                        text.Remove(text[0]);

                    }
                    string a = "";
                    for (int i = 0; i < to.X - from.X; i++)
                    {
                        a += message[0];
                        message = message.Remove(0, 1);
                    }

                    text.Add(a);
                    a = "";
                }
                if (text.Last().Length + message.Length < to.X - from.X && text.Last()[text.Last().Length - 1] != '\n')
                {
                    text[text.Count() - 1] = text.Last() + message;
                }
                else
                {
                    text.Add(message);
                }
            }
        }
    }


    class Program
    {
        
        static string ftpHost = "ftp://ftpupload.net";
        static string username = "if0_38982391";
        static string password = "VvWTyNLwWkWoY7";
        
        static string FtpListDirectory(string path="")
        {
            try { 
            string url = $"{ftpHost}/{path}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            

            return reader.ReadToEnd();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
            return "";

        }
        static string FtpListDirectory(bool onlyFiles, string path = "")
        {
            try
            {
                string url = $"{ftpHost}/{path}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(username, password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());


                FtpWebRequest request1 = (FtpWebRequest)WebRequest.Create(url);
                request1.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request1.Credentials = new NetworkCredential(username, password);

                FtpWebResponse response1 = (FtpWebResponse)request1.GetResponse();
                StreamReader reader1 = new StreamReader(response1.GetResponseStream());
                Console.WriteLine("1");
                string[] hgh = reader1.ReadToEnd().Split('\n');
                string[][] hgh2 = new string[][] { };
                int i = 0;
                int j = 0;
                Console.WriteLine("2");
                foreach (var ii in hgh)
                {
                    foreach (string jj in ii.Split(' ')) { 
                        hgh2[i][j++] = jj;
                        Console.WriteLine("s");
                    }
                    j = 0;
                    i++;
                    Console.WriteLine("i");
                }
                Console.WriteLine("ad");
                for(int o = 0; o < hgh2[0].Length; o++)
                {
                    Console.WriteLine(hgh2[o]);
                }
                
                Console.ReadLine();



                return reader.ReadToEnd();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
            return "";

        }

        static bool FtpDeleteFile(string path)
        {
            try
            {
                string url = $"{ftpHost}/{path}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(username, password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
            return false;

        }


        static bool FtpDownloadFile(string remotePath, string localPath="")
        {
            try
            {
                if (localPath.Length == 0) localPath = $"{Path.GetDirectoryName(Environment.ProcessPath)}/{Path.GetFileName(remotePath)}";

                string url = $"{ftpHost}/{remotePath}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(username, password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream ft = new FileStream(localPath, FileMode.Create))
                {
                    responseStream.CopyTo(ft);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                return false;
            }
            

        }


        static bool FtpUploadFile(string localPath, string remotePath)
        {
            try
            {
                
                string url = $"{ftpHost}/{remotePath}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);

                byte[] data = File.ReadAllBytes(localPath);
                request.ContentLength = data.Length;

                using (Stream requestStream = request.GetRequestStream()) { 
                    requestStream.Write(data, 0, data.Length);
                }


                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                return false;
            }


        }
        static readonly object lockObj = new object();
        public static uint Menu(IEnumerable<string> Action)
        {
            uint active = 0;
            while (true)
            {
                lock (lockObj)
                {
                    Console.SetCursorPosition(0, 0);
                    for (int i = 0; i < Action.Count(); i++)
                    {

                        if (i == active)
                            Console.WriteLine($" > {Action.ElementAt(i)}");
                        else
                            Console.WriteLine($"   {Action.ElementAt(i)}");
                    }
                }

                if (Console.KeyAvailable)
                {
                    
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                        active = (active > 0 ? --active : (uint) Action.Count() - 1);
                    else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                        active = (active < Action.Count() - 1) ? ++active : 0;
                    else if (key == ConsoleKey.Enter)
                    {
                        //Console.Clear();
                        return active;
                    }
                }
            }

        }
        static ConsoleWindow Window = new ConsoleWindow(new Point(40, 0), new Point(110, 25), lockObj, new object(), ConsoleColor.White, ConsoleColor.Green);
        static void Main(string[] args)
        {
            //Console.WriteLine("Files: ");
            //Console.WriteLine(FtpListDirectory());
            FtpListDirectory(true);
            Console.ReadLine();
            Thread b = new Thread(() => { while (true) { Window.Draw(); Thread.Sleep(100); } });
            b.Start();
            Window.DrawBackground();
            while (true)
            {
                int a = (int) Menu(new string[]{
                "Print files",
                "Delete file(not working)",
                "Download file",
                "Upload file",
                "Exit"
                });
                switch (a)
                {
                    case 0:{
                            
                            Window.WriteLine("Files: ");
                            Window.WriteLine(FtpListDirectory());
                            break;
                        }
                    case 1:{
                            Environment.Exit(0);
                            break;
                        }
                    case 2:{
                            string[] str = FtpListDirectory(true).Split('\n');
                            var h = Menu(str);
                            if (FtpDownloadFile(str[(int)h], "C:\\Users\\student\\DownloadFTP\\"))
                                Window.WriteLine("Succes!");
                            else
                                Console.WriteLine("ERROR!");
                            break;
                        }
                    case 3:{
                            Environment.Exit(0);
                            break;
                        }
                    case 4:{
                            Environment.Exit(0);
                            break;
                        }
                }



            }
            
            

            




            


            Console.WriteLine("Uploading file...");
            Console.WriteLine(FtpUploadFile("C:\\Users\\student\\TestFTP\\aboba.txt", "aboba/data.txt"));

        }



        
    }
}
