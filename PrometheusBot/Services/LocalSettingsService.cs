using System;
using System.IO;
using Newtonsoft.Json;

namespace PrometheusBot.Services
{
    public class LocalSettingsService
    {
        [JsonIgnore]
        public string Path { get; }

        public string DiscordToken { get; set; }
        public string ConnectionString { get; set; } = "Server=localhost;Port=3306;database=PrometheusDB;user id=PrometheusBot;password=PrometheusBot";
        public string CatApiKey { get; set; }
        public string DogApiKey { get; set; }

        public LocalSettingsService(string path)
        {
            Path = path;
        }

        public bool Load()
        {
            //Try to read the settings file, if it fails (because the file doesn't exist, is improperly formatted, etc.) return false

            try
            {
                string json = File.ReadAllText(Path);
                JsonConvert.PopulateObject(json, this);
                return true;
            }
            catch
            {
                return false;
            }
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
            catch
            {
                return false;
            }
        }
    }
}
