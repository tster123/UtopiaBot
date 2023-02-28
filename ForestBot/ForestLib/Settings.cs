using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace ForestLib
{
    public class Settings
    {
        private static Settings? _instance;
        private static readonly object _lock = new();

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new Settings("C:\\forestbot\\forest.settings");
                    }
                }
                return _instance;
            }
        }


        public readonly string DbServer,
            DbName,
            DbUsername,
            DbPassword,
            DiscordToken;

        public Settings(string file)
        {
            JObject json = JObject.Parse(File.ReadAllText(file));
            DbServer = json["DbServer"]?.Value<string>() ?? throw new InvalidOperationException();
            DbName = json["DbName"]?.Value<string>() ?? throw new InvalidOperationException();
            DbUsername = json["DbUsername"]?.Value<string>() ?? throw new InvalidOperationException();
            DbPassword = json["DbPassword"]?.Value<string>() ?? throw new InvalidOperationException();
            DiscordToken = json["DiscordToken"]?.Value<string>() ?? throw new InvalidOperationException();
        }

        public SqlConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = DbServer,
                InitialCatalog = DbName,
                UserID = DbUsername,
                Password = DbPassword
            };
        }
    }
}
