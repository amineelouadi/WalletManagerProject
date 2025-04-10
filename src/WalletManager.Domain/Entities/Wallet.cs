using System;
using System.Collections.Generic;

namespace WalletManager.Domain.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserId { get; set; } = default!;
        
        // Navigation properties
        public virtual User User { get; set; } = default!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
