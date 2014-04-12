using System.Configuration;
using System.Configuration.Provider;

namespace Rock.Configuration
{
    public class CategoryElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
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

        [ConfigurationProperty("providers", IsDefaultCollection = true)]
        public ProviderCollection Providers
        {
            get
            {
                return (ProviderCollection)this["providers"]; // questionable
            }
            set
            {
                this["providers"] = value;
            }
        }

        [ConfigurationProperty("throttlingRule")]
        public string ThrottlingRule
        {
            get
            {
                return (string)this["throttlingRule"];
            }
            set
            {
                this["throttlingRule"] = value;
            }
        }
    }
}