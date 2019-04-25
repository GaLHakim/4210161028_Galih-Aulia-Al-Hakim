using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDadu
{
    // define interface as Server/Client IDL.
    // implements T : IService<T> and share this type between server and client.
    public interface IMyFirstService : IService<IMyFirstService>
    {
        // Return type must be `UnaryResult<T>` or `Task<UnaryResult<T>>`.
        // If you can use C# 7.0 or newer, recommend to use `UnaryResult<T>`.
        UnaryResult<int> SumAsync(int x, int y);
        UnaryResult<int[]> RandomDadu(int player);
        UnaryResult<int[]> WinCon();
    }

    // implement RPC service to Server Project.
    // inehrit ServiceBase<interface>, interface
    public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        int hasilData;
        public static int[] highDadu = new int[2];
        public static int[] lowDadu = new int[2];

        /// <summary>
        /// Array ukurannya dijadikan 6
        /// Index 0 = Player yang memanggil
        /// Index 1 = Nilai dadu dari player yang memanggil
        /// Index 2 = Player dengan nilai tertinggi (Menang)
        /// Index 3 = Nilai dadu dari player yang tertinggi (Menang)
        /// Index 4 = Player dengan nilai terendah (Kalah)
        /// Index 5 = Nilai dadu dari player terendah (Kalah)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>

        public async UnaryResult<int[]> RandomDadu(int player)
        {
            int[] hasil = new int[6];
            var rnd = new Random();
            hasilData = rnd.Next(1, 6);
            hasil[0] = player;
            hasil[1] = hasilData;

            if (hasilData > highDadu[1])
            {
                highDadu[0] = player;
                highDadu[1] = hasilData;
                Console.WriteLine("highdadu Masuk: " + highDadu[1]);
            }

            if(lowDadu[0] == 0)
            {
                lowDadu[0] = player;
                lowDadu[1] = hasilData;
            }
            else if (hasilData < lowDadu[1])
            {
                lowDadu[0] = player;
                lowDadu[1] = hasilData;
            }
            Console.WriteLine("highdadu Gk masuk: " + highDadu[1]);
            return hasil;
        }

        public async UnaryResult<int[]> WinCon()
        {
            Console.WriteLine("highdadu Masuk: " + highDadu[1]);

            return highDadu;
        }

        // You can use async syntax directly.
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Logger.Debug($"Received:{x}, {y}");

            return x + y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            // setup MagicOnion and option.
            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);

            var server = new global::Grpc.Core.Server
            {
                Services = { service },
                Ports = { new ServerPort("localhost", 12345, ServerCredentials.Insecure) }
            };

            // launch gRPC Server.
            server.Start();

            // and wait.
            Console.ReadLine();
        }
    }
}
