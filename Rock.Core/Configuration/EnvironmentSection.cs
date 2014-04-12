using System.Configuration;

namespace Rock.Configuration
{
    public class EnvironmentSection : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Environment
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }
}
