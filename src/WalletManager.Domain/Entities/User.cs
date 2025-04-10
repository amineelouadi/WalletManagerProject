using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WalletManager.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
