using System.Configuration;

namespace Rock.Configuration
{
    public class LoggerSettingsSection : ConfigurationElement
    {
        [ConfigurationProperty("loggingLevel", DefaultValue = "Fatal", IsRequired = true)]
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

        [ConfigurationProperty("isLoggingEnabled", DefaultValue = true, IsRequired = false)]
        public bool IsLoggingEnabled
        {
            get
            {
                return (bool)this["isLoggingEnabled"];
            }
            set
            {
                this["isLoggingEnabled"] = value;
            }
        }

        //[ConfigurationProperty("auditProviderType", DefaultValue = "None", IsRequired = false)]
        //public string AuditProviderType
        //{
        //    get
        //    {
        //        return (string)this["auditProviderType"];
        //    }
        //    set
        //    {
        //        this["auditProviderType"] = value;
        //    }
        //}

        [ConfigurationProperty("categories", IsDefaultCollection = true)]
        public CategoryElementCollection Categories
        {
            get
            {
                return (CategoryElementCollection)base["categories"];
            }
            set
            {
                this["categories"] = value;
            }
        }

        [ConfigurationProperty("formatters", IsDefaultCollection = true)]
        public FormatterCollection Formatters
        {
            get
            {
                return (FormatterCollection)base["formatters"];
            }
            set
            {
                this["formatters"] = value;
            }
        }

        [ConfigurationProperty("throttlingRules", IsDefaultCollection = true)]
        public ThrottlingRuleCollection ThrottlingRules
        {
            get
            {
                return (ThrottlingRuleCollection)base["throttlingRules"];
            }
            set
            {
                this["throttlingRules"] = value;
            }
        }
    }
}