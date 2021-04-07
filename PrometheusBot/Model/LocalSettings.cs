using System;
using System.IO;
using Newtonsoft.Json;

namespace PrometheusBot.Model
{
    class LocalSettings
    {

        public static string Path { get; } = System.IO.Path.Combine(Program.Directory, "LocalSettings.json");

        public string DiscordToken { get; set; }
        public string ConnectionString { get; set; } = "Server=localhost;Port=3306;database=PrometheusDB;user id=PrometheusBot;password=PrometheusBot";
        public string CatApiKey { get; set; }
        public string DogApiKey { get; set; }

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
