using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("localhost", 2000);

            Stream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);

            Console.WriteLine(reader.ReadLine());

            stream.Flush();

            new Thread(() =>
            {
                Console.WriteLine("New Request {0} ", reader.ReadLine());

                Console.WriteLine("Waiting for exit");
                Console.ReadKey();
            }).Start();



            while (true)
            {
                string req = Console.ReadLine();

                if (req == "new")
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(req);
                    writer.Flush();
                }
            }
            client.Close();

        }
    }
}