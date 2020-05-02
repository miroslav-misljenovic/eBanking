using eBanking.BusinessModels;
using eBanking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace eBanking.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }

        private void PopulateTasks()
        {
            //string userId = _userManager.GetUserId(User);
            /*Transaction transactions = _dbContext.Users
                .Where(t => t.== 0);*/
            /*var currencies = user.Accounts.Select(a => a.Currency).ToList();
            var accounts = user.Accounts.ToList();*/

            /*
            to.Balance += (decimal) transaction.Amount;
            from.Balance -= (decimal) transaction.Amount;*/

            /*ViewData["Accounts"] = accounts;
            ViewData["Currencies"] = new SelectList(currencies, "Id", "Name");*/
        }
        public IActionResult Index()
        {
            var tasks = _dbContext.Transactions
                .Where(t => t.Status == TransactionStatus.PENDING)
                .Select(t => new Models.Task 
                {
                    Id = t.Id,
                    Sender = t.AccountFrom.ApplicationUser.UserName,
                    Receiver = t.AccountTo.ApplicationUser.UserName,
                    CurrentBalance = t.AccountFrom.Balance,
                    Amount = t.Amount,
                    Currency = t.AccountFrom.Currency.Name,
                })
                .ToList();
            return View(tasks);
        }

        public IActionResult ApproveTransaction(int taskId)
        {
            var transaction = _dbContext.Transactions
                .Include(t => t.AccountFrom)
                .Include(t => t.AccountTo)
                .First(t => t.Id == taskId);

            transaction.AccountTo.Balance += (decimal)transaction.Amount;
            transaction.AccountFrom.Balance -= (decimal)transaction.Amount;

            transaction.Status = TransactionStatus.APROVED;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult RejectTransaction(int taskId)
        {
            var transaction = _dbContext.Transactions.First(t => t.Id == taskId).Status = TransactionStatus.CANCELED;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}