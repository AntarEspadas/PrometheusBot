using System;
using System.IO;
using Newtonsoft.Json;

namespace PrometheusBot.Model
{
    class LocalSettings
    {

        public static string Path { get; } = System.IO.Path.Combine(Program.Directory, "LocalSettings.json");

        public string DiscordToken { get; set; }
        public string MysqlUsername { get; set; } = "PrometheusBot";
        public string MysqlPassword { get; set; } = "PrometheusBot";
        public string MysqlAddress { get; set; } = "localhost";
        public int MysqlPort { get; set; } = 3306;

        private LocalSettings()
        {

        }

        public static LocalSettings Load()
        {
            //Try to read the settings file, if it fails (because the file doesn't exist, is improperly formatted, etc.) LocalSettings instance with default values
            LocalSettings localSettings = null;

            try
            {
                string json = File.ReadAllText(Path);
                localSettings = JsonConvert.DeserializeObject<LocalSettings>(json);
            }
            catch { }

            localSettings ??= new();

            return localSettings;
        }

        public bool Save()
        {
            //Attempt to save settings, return true in case of success and false in case of failure
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            try
            {
                File.WriteAllText(Path, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
