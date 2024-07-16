using Atlantic.Data.Models.Auth;
using Atlantic.Data.Models.Static;
using Atlantic.Data.Repositories;
using Atlantic.Data.Services;
using Microsoft.AspNetCore.Identity;
using MongoDB.Entities;

namespace Atlantic.Data.Seeds
{
    public interface IAppSeederInitial
    {
        Task Initialize();
    }
    public class AppSeederInitial : IAppSeederInitial
    {
        private readonly string logFilename = "logs/AppSeederInitial.txt";
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly ICountryService _countryService;
        public AppSeederInitial(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ICountryService countryService,
            IUnitOfWork unitOfWork,
            IMailService mailService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _countryService = countryService;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        public async Task Initialize()
        {
            await Log("Started ");
            //var request = new WelcomeRequest { ToEmail = "rronyy@gmail.com", UserName = "Application Started" };
            //await _mailService.SendWelcomeEmailAsync(request);
            if (!_roleManager.Roles.Any())
            {
                var roles = new List<string> { "Admin", "Executive", "Manager", "User" };
                foreach (var role in roles)
                {
                    var r = new ApplicationRole { Name = role };
                    try
                    {
                        IdentityResult result = await _roleManager.CreateAsync(r);

                        if (result.Succeeded)
                        {

                        }
                        else
                        {
                            throw new Exception(result.Errors.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                }
            }


            try
            {

                if (!_userManager.Users.Any())
                {
                    await Log("No users found");
                    var user = new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = "admin@example.com",
                        PhoneNumber = "+8801670502610",
                        PasswordValidity = DateTime.Now.AddDays(90),
                        Status = true,
                        IsSystemAdmin = true,
                        UserGroup = "Admin",
                        UserType = "Admin",
                        UserRole = "Admin"
                    };

                    IdentityResult result = await _userManager.CreateAsync(user, "Asdf@1123");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                }

                if (_userManager.Users.Any(x => x.Email.Equals("admin@dsmvisacard.com") && x.IsSystemAdmin != true))
                {
                    var adminUser = await _userManager.FindByEmailAsync("admin@dsmvisacard.com");
                    if (adminUser != null)
                    {
                        adminUser.IsSystemAdmin = true;
                        await _userManager.UpdateAsync(adminUser);
                    }
                }

                if (!DB.Queryable<Country>().Any())
                {
                    //var content = System.IO.File.ReadAllText("StaticData/countries.json");
                    //var data = JsonConvert.DeserializeObject<IList<Country>>(content);
                    //await _unitOfWork.CountryRepository.InsertRange(data);
                    await _countryService.Create(new Country
                    {
                        Name = "Malaysia",
                        Code = "MY",
                        DialingCode = "+60",
                        Currency = "MYR",
                        CurrencyPrint = "RM",
                        Rate = 1
                    });

                    await _countryService.Create(new Country
                    {
                        Name = "Bangladesh",
                        Code = "BD",
                        DialingCode = "+880",
                        Currency = "BDT",
                        CurrencyPrint = "BDT",
                        Rate = 25.50
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Log(ex.Message);
            }
        }

        async Task Log(string message)
        {
            await File.WriteAllLinesAsync(logFilename, new string[] { message, DateTime.Now.ToString(), Environment.NewLine });

        }
    }
}
