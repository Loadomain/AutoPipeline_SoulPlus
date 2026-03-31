namespace SouthPlusDownloader
{
    public class TaskConfig
    {
        public int BoardId { get; set; }
        public int StartItem { get; set; }
        public int EndItem { get; set; }
        public int MaxPrice { get; set; }
        public string Cookie { get; set; } = string.Empty;
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
        public string[] Platforms { get; set; } = [];
        public int MaxDelayMs { get; set; } = 3000;
        public int MinDelayMs { get; set; } = 1000;
        public string CustomCodeKeywords { get; set; } = string.Empty;
        public string CustomPasswordKeywords { get; set; } = string.Empty;

        public static TaskConfig LoadCredentials(string filename = "credentials.json")
        {
            if (File.Exists(filename))
            {
                try {
                    var text = File.ReadAllText(filename);
                    return System.Text.Json.JsonSerializer.Deserialize<TaskConfig>(text);
                } catch { }
            }
            return new TaskConfig();
        }

        public void SaveCredentials(string filename = "credentials.json")
        {
            File.WriteAllText(filename, System.Text.Json.JsonSerializer.Serialize(this));
        }
    }
}
