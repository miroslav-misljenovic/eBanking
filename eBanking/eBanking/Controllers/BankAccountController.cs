using eBanking.Data;
using eBanking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using eBanking.BusinessModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eBanking.Controllers
{
    public class BankAccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BankAccountController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) 
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: /<controller>/
        private void PopulateCurrencies()
        {
            var currencies = _dbContext.Currencies.ToList();
            ViewData["Currencies"] = new SelectList(currencies, "Id", "Name");
        }
        public IActionResult Index()
        {
            PopulateCurrencies();
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewAccount (CreateBankAccount account) {
            PopulateCurrencies();
            string userId = _userManager.GetUserId(User);
            
            ApplicationUser user = _dbContext.Users.Include(u => u.Accounts).First(s => s.Id == userId);
            if(user.Accounts.Exists(c => c.CurrencyId == account.CurrencyId))
            {
                ViewData["Message"] = "You already have account of this currency!";
                return View("Index");
            }

            _dbContext.BankAccounts.Add(new BankAccount { CurrencyId = account.CurrencyId, UserId = user.Id });
            _dbContext.SaveChanges();
            ViewData["Message"] = "Account created!";

            return View("Index");
        }
    }
  
}
