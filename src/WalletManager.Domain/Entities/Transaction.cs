using System;
using WalletManager.Domain.Enums;

namespace WalletManager.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }
        public string? Reference { get; set; }
        
        // Navigation properties
        public virtual Wallet Wallet { get; set; } = default!;
    }
}
