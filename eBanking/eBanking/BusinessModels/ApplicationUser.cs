using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace eBanking.BusinessModels
{
    public class ApplicationUser : IdentityUser
    {
        public List<BankAccount> Accounts { get; set; }

    }
}
