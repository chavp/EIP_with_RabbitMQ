using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathRPCClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var timeWatcher = Stopwatch.StartNew();

            using (var rpcClient = new MateRPCApi())
            {
                for (int i = 10; i < 40; i++)
                {
                    //Console.WriteLine(" [x] Requesting fib({0})", i);
                    var response = rpcClient.Fib(i);
                    Console.WriteLine(" [.] Got {0} - Elapsed {1}", response.N, response.Elapsed);
                }
            }


            Console.WriteLine(" [.] Fib 10 to 40 - Elapsed {0}", timeWatcher.Elapsed);

            Console.ReadLine();
        }
    }
}
