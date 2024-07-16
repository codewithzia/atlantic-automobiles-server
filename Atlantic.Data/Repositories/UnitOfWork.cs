
using Atlantic.Data.Hubs;
using Atlantic.Data.Models;
using Atlantic.Data.Models.Auth;
using Atlantic.Data.Models.Static;
using Atlantic.Data.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace Atlantic.Data.Repositories
{
    public interface IUnitOfWork
    {

        IHttpContextAccessorService HttpAccessor();
        IHubContext<NotificationHub> NotificationHub();
        IStaticRepository<Country> CountryRepository { get; }
        IRepository<UserProfile> UserProfileRepository { get; }
        IRepository<Customer> CustomerRepository { get; }
        IRepository<Permission> PermissionRepository { get; }

        string GetUserId();
        string EnvironmentName();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IHttpContextAccessorService _httpAccessor;

        private readonly IHubContext<NotificationHub> _hub;
        private readonly IWebHostEnvironment _hostingEnv;
        private IStaticRepository<Country>? _countryRepository;
        private IRepository<UserProfile>? _userProfileRepository;
        private IRepository<Customer>? _customerRepository;
        private IRepository<Permission>? _permissionRepository;


        public UnitOfWork(IHttpContextAccessorService httpAccessor, IHubContext<NotificationHub> hub, IWebHostEnvironment hostingEnv)
        {
            _httpAccessor = httpAccessor;
            _hub = hub;
            _hostingEnv = hostingEnv;
        }
        public IHubContext<NotificationHub> NotificationHub()
        {
            return _hub;
        }

        public string EnvironmentName()
        {
            return _hostingEnv.EnvironmentName;
        }
        public async Task NotifyToCheckBalance()
        {
            await NotificationHub().Clients.All.SendAsync("TransferData", new
            {
                CheckBalance = true
            });
        }

        public IHttpContextAccessorService HttpAccessor()
        {
            return _httpAccessor;
        }

        public string GetUserId()
        {
            return _httpAccessor.CurrentUserId();
        }

        public IStaticRepository<Country> CountryRepository
        {
            get { return _countryRepository = _countryRepository ?? new StaticRepository<Country>(_httpAccessor); }
        }

        public IRepository<UserProfile> UserProfileRepository
        {
            get { return _userProfileRepository = _userProfileRepository ?? new Repository<UserProfile>(_httpAccessor); }
        }


        public IRepository<Customer> CustomerRepository
        {
            get { return _customerRepository = _customerRepository ?? new Repository<Customer>(_httpAccessor); }
        }

        public IRepository<Permission> PermissionRepository
        {
            get { return _permissionRepository = _permissionRepository ?? new Repository<Permission>(_httpAccessor); }
        }
    }

}
