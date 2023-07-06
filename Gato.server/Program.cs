using Gato.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gato.server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PlayerMove accountServer = new PlayerMove();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Manager initialize...");
            accountServer.Initialize();
            Console.WriteLine("Press any key for stop..");
            Console.ReadLine();
            accountServer.Uninitialize();
        }
    }
}
