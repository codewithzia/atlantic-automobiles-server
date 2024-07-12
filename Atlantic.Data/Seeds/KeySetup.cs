using Atlantic.Data.Models.Static;
using MongoDB.Entities;

namespace Atlantic.Data.Seeds
{
    public interface IKeySetup
    {
        Task Setup();
    }
    public class KeySetup : IKeySetup
    {
        private readonly string _logFilePath = "logs/KeySetupErrors.txt";

        public async Task Setup()
        {
            try
            {
                await DB.Index<Country>()
                   .Key(x => x.Name, KeyType.Descending)
                   .Key(x => x.Code, KeyType.Descending)
                    .Option(o =>
                    {
                        o.Background = false;
                        o.Unique = true;
                    })
                    .CreateAsync();

            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync(_logFilePath, "Error: " + ex.Message + Environment.NewLine);
            }
        }
    }
}
