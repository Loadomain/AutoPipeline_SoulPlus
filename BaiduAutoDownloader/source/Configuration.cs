using System;
using System.IO;
using Newtonsoft.Json;

namespace BaiduAutoDownloader
{
    public enum ApiAuthMode
    {
        PublicPool,
        Custom
    }

    public class Configuration
    {
        public ApiAuthMode ApiMode { get; set; } = ApiAuthMode.PublicPool;

        public string[] PublicKeys { get; set; } = new string[] 
        { 
            "iYCeC9g08h5vuP9UqvPHKKSVrKFXGa1v", // AList
            "q8WE4EpCsau1oS0Mwtg1cVzsqNDzIeZT", // bypy
            "jX6XFDr6aT20z7T067c29lH2vY21G0fD", // other generic key
        };

        public int CurrentPoolIndex { get; set; } = 0;

        public string CustomAppKey { get; set; } = "";

        // Dynamically get the currently active AppKey based on mode
        [JsonIgnore]
        public string AppKey 
        {
            get 
            {
                if (ApiMode == ApiAuthMode.Custom && !string.IsNullOrWhiteSpace(CustomAppKey))
                    return CustomAppKey;
                
                if (PublicKeys != null && PublicKeys.Length > 0)
                    return PublicKeys[CurrentPoolIndex % PublicKeys.Length];
                
                return "";
            }
        }

        public string AccessToken { get; set; } = "";
        public string TargetDirectory { get; set; } = "";

        public void RotateToNextPublicKey()
        {
            if (ApiMode == ApiAuthMode.PublicPool && PublicKeys != null && PublicKeys.Length > 0)
            {
                CurrentPoolIndex = (CurrentPoolIndex + 1) % PublicKeys.Length;
                AccessToken = ""; // Invalidate bad token on rotate
                Save();
            }
        }
        
        private static readonly string ConfigPath = "config.json";

        public static Configuration Load()
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<Configuration>(json) ?? new Configuration();
            }
            return new Configuration();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
