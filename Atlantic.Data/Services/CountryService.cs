using Atlantic.Data.Contexts;
using Atlantic.Data.Models.Static;
using MongoDB.Entities;

namespace Atlantic.Data.Services
{
    public interface ICountryService
    {
        Task<Country> Create(Country country);

    }
    public class CountryService : ICountryService
    {
        protected readonly MongoContext _context;
        private readonly IHttpContextAccessorService _httpAccessor;
        public CountryService(IHttpContextAccessorService httpAccessor)
        {
            _httpAccessor = httpAccessor;
            _context = new MongoContext(httpAccessor);
        }

        public async Task<Country> Create(Country Country)
        {

            var r = await DB.Index<Country>()
                .Key(x => x.Name, KeyType.Descending)
                .Key(x => x.Code, KeyType.Descending)
                 .Option(o =>
                     {
                         o.Background = false;
                         o.Unique = true;
                     })
                 .CreateAsync();
            await Country.SaveAsync();
            return Country;
        }
    }
}
