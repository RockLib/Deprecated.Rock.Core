using System.Configuration;

namespace Rock.Configuration
{
    public class RockFrameworkSection : ConfigurationSection
    {
        [ConfigurationProperty("applicationId", IsRequired = true)]
        public int ApplicationId
        {
            get
            {
                return (int)this["applicationId"];
            }
            set
            {
                this["applicationId"] = value;
            }
        }

        [ConfigurationProperty("environment", IsRequired = true)]
        public EnvironmentSection EnvironmentSection
        {
            get
            {
                return (EnvironmentSection)this["environment"];
            }
            set
            {
                this["environment"] = value;
            }
        }

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