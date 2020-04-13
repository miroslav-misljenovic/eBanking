using eBanking.BusinessModels;
using eBanking.Data;
using eBanking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eBanking.Controllers
{
    public class ConversionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        public ConversionController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }
        // GET: /<controller>/
        private void PopulateCurrencies()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _dbContext.Users
                .Include("Accounts.Currency")
                .First(s => s.Id == userId);
            var currencies = user.Accounts.Select(a => a.Currency).ToList();
            var accounts = user.Accounts.ToList();

            ViewData["Accounts"] = accounts;
            ViewData["Currencies"] = new SelectList(currencies, "Id", "Name");
        }
        public IActionResult Index()
        {
            PopulateCurrencies();
            return View();
        }

        public IActionResult ConvertMoney(Conversion conversion)
        {
            PopulateCurrencies();
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _dbContext
                .Users
                .Include("Accounts.Currency")
                .First(s => s.Id == userId);

            if (conversion.CurrencyIdFrom == conversion.CurrencyIdTo)
            {
                ViewData["Message"] = "Select different accounts!";
                return View("Index");
            }

            BankAccount from = user.Accounts.First(a => a.CurrencyId == conversion.CurrencyIdFrom);
            BankAccount to = user.Accounts.First(a => a.CurrencyId == conversion.CurrencyIdTo);

            if (conversion.Amount > from.Balance)
            {
                ViewData["Message"] = "Not enough resources";
                return View("Index");
            }

            to.Balance += conversion.Amount * (decimal)(from.Currency.Rate / to.Currency.Rate);
            from.Balance -= conversion.Amount;

            _dbContext.SaveChanges();
            ViewData["Message"] = "Conversion completed!";
            return View("Index");
        }
    }
}
