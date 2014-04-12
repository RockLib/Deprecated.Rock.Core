using System.Configuration;

namespace Rock.Configuration
{
    public class RockFrameworkSection : ConfigurationSection
    {
        [ConfigurationProperty("logger", IsRequired = false)]
        public LoggerSectionGroup LoggerSettings
        {
            get
            {
                return (LoggerSectionGroup)this["logger"];
            }
            set
            {
                this["logger"] = value;
            }
        }
    }
}