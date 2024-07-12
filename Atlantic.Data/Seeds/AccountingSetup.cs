using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlantic.Data.Models.Auth;
using Atlantic.Data.Repositories;

namespace Atlantic.Data.Seeds
{
    public interface IAccountingSetup
    {
        Task Initialize();
    }
    public class AccountingSetup : IAccountingSetup
    {

        private readonly string logFilename = "logs/AppSeederInitial.txt";
        private UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public AccountingSetup(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task Initialize()
        {
            await Log("Started ");
        }

        async Task Log(string message)
        {
            await File.WriteAllLinesAsync(logFilename, new string[] { message, DateTime.Now.ToString(), Environment.NewLine });

        }
    }
}