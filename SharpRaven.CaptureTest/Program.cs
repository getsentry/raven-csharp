using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRaven.CaptureTest {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Initializing RavenClient.");
            RavenClient rc = new RavenClient("https://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739");

            PrintInfo("Sentry Uri: " + rc.CurrentDSN.SentryURI);
            PrintInfo("Port: " + rc.CurrentDSN.Port);
            PrintInfo("Public Key: " + rc.CurrentDSN.PublicKey);
            PrintInfo("Private Key: " + rc.CurrentDSN.PrivateKey);
            PrintInfo("Project ID: " + rc.CurrentDSN.ProjectID);

            Console.WriteLine("Causing division by zero exception.");
            try {
                Program.PerformDivideByZero();
                Console.WriteLine("Failed.");
            } catch (Exception e) {
                Console.WriteLine("Captured: " + e.Message);
                int id = rc.CaptureEvent(e);
                Console.WriteLine("Sent packet: " + id);
            }

            Console.ReadLine();
        }

        static void PerformDivideByZero() {
            int i2 = 0;
 	        int i = 10 / i2;
        }

        static void PrintInfo(string info) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[INFO] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(info);
        }
    }
}
