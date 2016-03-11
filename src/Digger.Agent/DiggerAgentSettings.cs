using System.Configuration;

namespace Digger.Agent
{
    public class DiggerAgentSettings
    {
        public string ConfigFilePath { get; private set; }

        private DiggerAgentSettings() { }

        public static DiggerAgentSettings ReadFromAppSettings()
        {
            return new DiggerAgentSettings
            {
                ConfigFilePath = GetString("app:config:filePath")
            };
        }

        private static string GetString(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}