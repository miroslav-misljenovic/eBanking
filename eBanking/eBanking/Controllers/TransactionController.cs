using eBanking.BusinessModels;
using eBanking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eBanking.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;
        public TransactionController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));

        }
        private void PopulateAccounts()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _dbContext.Users
                .Include("Accounts.Currency")
                .First(s => s.Id == userId);
            var currencies = user.Accounts.Select(a => a.Currency).ToList();
            //var accounts = user.Accounts.ToList();

            //ViewData["Accounts"] = accounts;
            ViewData["Currencies"] = new SelectList(currencies, "Id", "Name");
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            PopulateAccounts();
            return View();
        }

        public IActionResult SendMoney(Models.Transaction transaction)
        {
            PopulateAccounts();
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _dbContext
                .Users
                .Include("Accounts.Currency")
                .First(s => s.Id == userId);
            
            BankAccount from = user.Accounts.First(a => a.CurrencyId == transaction.SenderCurrencyId);
            BankAccount to = _dbContext
                .BankAccounts
                .Where(a => a.UserId != user.Id 
                        && a.CurrencyId == from.CurrencyId 
                        && a.Id == transaction.RecipientAccount)
                .FirstOrDefault();

            if(to == null)
            {
                ViewData["Message"] = "Invalid recipient ID!";
                return View("Index");
            }

            if (transaction.Amount > (double)from.Balance)
            {
                ViewData["Message"] = "Not enough resources!";
                return View("Index");
            }
            if (_dbContext.Transactions
                .Where(t => t.AccountFrom.Id == from.Id && t.Status == TransactionStatus.PENDING)
                .Any())
            {
                ViewData["Message"] = "Already exists pending transaction from that bank account!";
                return View("Index");
            }

            Transaction t = new Transaction();
            t.AccountFromId = from.Id;
            t.AccountToId = to.Id;
            t.Amount = transaction.Amount;
            t.Status = TransactionStatus.PENDING;
            _dbContext.Transactions.Add(t);
            _dbContext.SaveChanges();

            ViewData["Message"] = "Transfer transaction has been successfully forwarded to the manager!";
            return View("Index");
        }

    }
}
