using System;
using System.Configuration;

namespace SharpRaven.Nancy
{
    public class Configuration : SharpRaven.Configuration
    {
        private const string pipelineNameKey = "pipelineName";
        private const string captureExceptionOnErrorKey = "captureExceptionOnError";
        private static readonly Configuration settings =
            ConfigurationManager.GetSection("sharpRaven") as Configuration;

        public static Configuration Settings
        {
            get 
            { 
                return settings; 
            }
        }

        public string NancyContextDataSlot
        {
            get 
            { 
                return "SharpRaven.Nancy.NancyContext"; 
            }
        }

        [ConfigurationProperty(pipelineNameKey, IsKey = true)]
        public PipelineNameElement PipelineName
        {
            get 
            { 
                return (PipelineNameElement)base[pipelineNameKey]; 
            }
        }

        public class PipelineNameElement : ConfigurationElement
        {
            [ConfigurationProperty("value", DefaultValue = "SharpRaven.Nancy")]
            public String Value
            {
                get
                {
                    return (string)this["value"];
                }
                set
                {
                    this["value"] = value;
                }
            }
        }

        [ConfigurationProperty(captureExceptionOnErrorKey, IsKey = true)]
        public CaptureExceptionOnErrorElement CaptureExceptionOnError
        {
            get 
            { 
                return (CaptureExceptionOnErrorElement)base[captureExceptionOnErrorKey]; 
            }
        }

        public class CaptureExceptionOnErrorElement : ConfigurationElement
        {
            [ConfigurationProperty("value", DefaultValue = "true")]
            public bool Value
            {
                get
                {
                    return (bool)this["value"];
                }
                set
                {
                    this["value"] = value;
                }
            }
        }
    }
}
