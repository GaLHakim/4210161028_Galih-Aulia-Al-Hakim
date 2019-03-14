using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace ServerCsgo
{
    class Program
    {
        static TcpListener server;
        static int count = 0;
        static readonly object o = new object();

        static void Main(string[] args)
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ip = new IPAddress(host.AddressList.First().GetAddressBytes());

            server = new TcpListener(ip, 2000);
            server.Start();

            bool exit = false;

            Console.WriteLine("Server started on port 2000");

            while (!exit)
            {
                // here pending requests are in a queue.
                if (server.Pending())
                {
                    new Thread(Program.ServerService).Start();
                }
            }

            Console.ReadKey();
            server.Stop();

        }

        public static void ServerService()
        {
            Socket socket = server.AcceptSocket();

            Console.WriteLine("Connected " + socket.RemoteEndPoint);

            Stream stream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);

            writer.AutoFlush = true;

            writer.Write("Initial Number {0}", GetNumber());

            bool client = false;

            new Thread(() =>
            {
                while (!client)
                {
                    string s = "";
                    s = reader.ReadLine();

                    if (s == "req")
                    {
                        writer.Write(GetNumber());
                        writer.Flush();
                    }
                }
            }).Start();
        }

        private static int GetNumber()
        {
            // here sleep time gets activated after one client leaves the lock block.
            // so if a first client gets short long sleep time and second one gets short, still the second one has to wait till the first one finishes.

            Random r = new Random();
            int time = r.Next(200, 2000);
            Console.WriteLine("Sleeping time : {0}", time);

            lock (o)
            {
                Thread.Sleep(time);
                return ++count;
            }
        }
    }
}
