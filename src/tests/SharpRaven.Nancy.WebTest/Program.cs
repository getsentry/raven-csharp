using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRaven.Nancy.WebTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri("http://localhost:3579")))
            {
                host.Start();

                Console.WriteLine("Nancy is up and runnig. Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}
