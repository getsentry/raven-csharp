using System;
using System.Configuration;

namespace SharpRaven
{
    public class Configuration : ConfigurationSection
    {
        private const string dsnKey = "dsn";
        private static readonly Configuration settings =
            ConfigurationManager.GetSection("sharpRaven") as Configuration;

        public static Configuration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty(dsnKey, IsKey = true)]
        public DsnElement Dsn
        {
            get { return (DsnElement)base[dsnKey]; }
        }

        public class DsnElement : ConfigurationElement
        {
            [ConfigurationProperty("value")]
            public String Value
            {
                get
                {
                    return (String)this["value"];
                }
                set
                {
                    this["value"] = value;
                }
            }
        }
    }
}
