namespace Atlantic.Data.Models.Settings
{
    public class DbSettings
    {
        public string AppName { get; set; }
        public string DefaultEmail { get; set; }
        public string SiteLink { get; set; }
        public string ConnectionStrings { get; set; }
        public DatabaseSettings Database { get; set; }
        public string DocLocation { get; set; }
        public string SiteName { get; set; }

        public class DatabaseSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
