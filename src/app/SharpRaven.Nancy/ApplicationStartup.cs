using System;
using System.Configuration;
using Nancy.Bootstrapper;
using Nancy;
using System.Threading;

namespace SharpRaven.Nancy
{
    public class ApplicationStartup : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            var sharpRaven = new PipelineItem<Func<NancyContext, Exception, Response>>(
                Configuration.Settings.PipelineName.Value, (context, exception) =>
                {
                    Thread.SetData(
                       Thread.GetNamedDataSlot("SharpRaven.Nancy.NancyContext"),
                       context);

                    if (Configuration.Settings.CaptureExceptionOnError.Value)
                    {
                        IRavenClient client = new RavenClient(context);

                        client.CaptureException(exception);
                    }

                    return null;
                });

            pipelines.OnError.AddItemToStartOfPipeline(sharpRaven);
        }
    }
}


