using System;

using Nancy.Hosting.Self;

namespace SharpRaven.Nancy.UnitTest
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
