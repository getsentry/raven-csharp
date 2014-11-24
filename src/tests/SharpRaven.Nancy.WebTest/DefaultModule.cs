using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRaven.Nancy.WebTest
{
    public class DefaultModule : NancyModule
    {
        private static void DivideByZero(int stackFrames = 10)
        {
            if (stackFrames == 0)
            {
                var a = 0;
                var b = 1 / a;
            }
            else
            {
                DivideByZero(--stackFrames);
            }
        }

        public DefaultModule()
        {
            Get["/"] = _ => {
                return View["default.html"];
            };

            Post["/"] = _ =>
            {
                string exceptionId = String.Empty;

                try
                {
                    DivideByZero();
                }
                catch (Exception ex)
                {
                    IRavenClient ravenClient = new RavenClient(this.Context);
                    exceptionId = ravenClient.CaptureException(ex);
                }

                return View["error.html", new { ExceptionId = exceptionId }];
            };
        }
    }
}
