using System.IO;
using Newtonsoft.Json;
using VkNewsTracker.Common.Models;

namespace VkNewsTracker.Common.Services
{
    public class SettingsService
    {
        private readonly string _path;
        public ApplicationSettings ApplicationSettings { get; set; }

        public SettingsService(string path)
        {
            _path = path;
        }

        public void Load()
        {
            var settingsFileContent = "{}";
            if (File.Exists(_path))
            {
                settingsFileContent = File.ReadAllText(_path);
            }
            ApplicationSettings = (JsonConvert.DeserializeObject<ApplicationSettings>(settingsFileContent));
        }

        public void Save()
        {
            var settingsFileContent = JsonConvert.SerializeObject(ApplicationSettings);
            File.WriteAllText(_path, settingsFileContent);
        }
    }
}
