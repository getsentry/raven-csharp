using System;

namespace SharpRaven.CaptureTest
{
    class Program
    {
        const string dsnUrl = "https://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";
        static RavenClient ravenClient;

        static void Main(string[] args)
        {
            setup();
            testWithStacktrace();
            testWithoutStacktrace();

            Console.ReadLine();
        }

        static void setup()
        {
            Console.WriteLine("Initializing RavenClient.");
            ravenClient = new RavenClient(dsnUrl);
            ravenClient.Logger = "C#";
            ravenClient.LogScrubber = new Logging.LogScrubber();

            PrintInfo("Sentry Uri: " + ravenClient.CurrentDSN.SentryURI);
            PrintInfo("Port: " + ravenClient.CurrentDSN.Port);
            PrintInfo("Public Key: " + ravenClient.CurrentDSN.PublicKey);
            PrintInfo("Private Key: " + ravenClient.CurrentDSN.PrivateKey);
            PrintInfo("Project ID: " + ravenClient.CurrentDSN.ProjectID);
        }

        static void testWithoutStacktrace()
        {
            Console.WriteLine("Send exception without stacktrace.");
            var id = ravenClient.CaptureException(new Exception("Test without a stacktrace."));
            Console.WriteLine("Sent packet: " + id);
        }

        static void testWithStacktrace()
        {
            Console.WriteLine("Causing division by zero exception.");
            try
            {
                PerformDivideByZero();
            }
            catch (Exception e)
            {
                Console.WriteLine("Captured: " + e.Message);
                var id = ravenClient.CaptureException(e);
                Console.WriteLine("Sent packet: " + id);
            }
        }

        static void PerformDivideByZero()
        {
            int i2 = 0;
            int i = 10 / i2;
        }

        static void PrintInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[INFO] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(info);
        }
    }
}
