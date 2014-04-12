using System.Configuration;

namespace Rock.Configuration
{
    public class ProviderElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string ProviderType
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        [ConfigurationProperty("formatter", IsRequired = false)]
        public string Formatter
        {
            get
            {
                return (string)this["formatter"];
            }
            set
            {
                this["formatter"] = value;
            }
        }

        [ConfigurationProperty("loggingLevel", IsRequired = false)]
        public string LoggingLevel
        {
            get
            {
                return (string)this["loggingLevel"];
            }
            set
            {
                this["loggingLevel"] = value;
            }
        }
     }
}