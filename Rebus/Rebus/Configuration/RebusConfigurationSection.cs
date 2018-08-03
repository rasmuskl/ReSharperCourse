using System;
using System.Configuration;

namespace Rebus.Configuration
{
    public class RebusConfigurationSection : ConfigurationSection
    {
        const string MappingsCollectionPropertyName = "endpoints";
        const string RijndaelCollectionPropertyName = "rijndael";
        const string InputQueueAttributeName = "inputQueue";
        const string ErrorQueueAttributeName = "errorQueue";
        const string WorkersAttributeName = "workers";
        const string configSectionName = "rebus";

        [ConfigurationProperty(RijndaelCollectionPropertyName)]
        public RijndaelSection RijndaelSection
        {
            get { return (RijndaelSection)this[RijndaelCollectionPropertyName]; }
            set { this[RijndaelCollectionPropertyName] = value; }
        }

        [ConfigurationProperty(MappingsCollectionPropertyName)]
        public MappingsCollection MappingsCollection
        {
            get { return (MappingsCollection)this[MappingsCollectionPropertyName]; }
            set { this[MappingsCollectionPropertyName] = value; }
        }

        [ConfigurationProperty(InputQueueAttributeName)]
        public string InputQueue
        {
            get { return (string)this[InputQueueAttributeName]; }
            set { this[InputQueueAttributeName] = value; }
        }

        [ConfigurationProperty(ErrorQueueAttributeName)]
        public string ErrorQueue
        {
            get { return (string)this[ErrorQueueAttributeName]; }
            set { this[ErrorQueueAttributeName] = value; }
        }

        [ConfigurationProperty(WorkersAttributeName)]
        public int? Workers
        {
            get { return (int?)this[WorkersAttributeName]; }
            set { this[WorkersAttributeName] = value; }
        }

        public const string ExampleSnippetForErrorMessages = @"

    <rebus inputQueue=""this.is.my.input.queue"" errorQueue=""this.is.my.error.queue"" workers=""5"">
        <rijndael iv=""OLYKdaDyETlu7NbDMC45dA=="" key=""oA/ZUnFsR9w1qEatOByBSXc4woCuTxmR99tAuQ56Qko=""/>
        <endpoints>
            <add messages=""Name.Of.Assembly"" endpoint=""message_owner_1""/>
            <add messages=""Namespace.ClassName, Name.Of.Another.Assembly"" endpoint=""message_owner_2""/>
        </endpoints>
    </rebus>
";

        public static RebusConfigurationSection LookItUp()
        {
            var section = ConfigurationManager.GetSection(configSectionName);

            if (section == null || !(section is RebusConfigurationSection))
            {
                throw new ConfigurationErrorsException(@"Could not find configuration section named 'rebus' (or else
the configuration section was not of the Rebus.Configuration.RebusConfigurationSection type?)

Please make sure that the declaration at the top matches the XML element further down. And please note
that it is NOT possible to rename this section, even though the declaration makes it seem like it.");
            }

            return (RebusConfigurationSection)section;
        }

        public static TValue GetConfigurationValueOrDefault<TValue>(Func<RebusConfigurationSection, TValue> getConfigurationValue, TValue defaultValue)
        {
            var section = ConfigurationManager.GetSection(configSectionName);

            if (!(section is RebusConfigurationSection)) return defaultValue;

            var configurationValue = getConfigurationValue((RebusConfigurationSection)section);

            if (configurationValue == null) return defaultValue;

            return configurationValue;
        }
    }
}