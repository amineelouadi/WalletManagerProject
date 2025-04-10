using System;
using System.Collections.Generic;
using WalletManager.Domain.Enums;

namespace WalletManager.Application.DTOs
{
    public class DashboardDto
    {
        public decimal TotalBalance { get; set; }
        public int WalletCount { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpense;
        public List<WalletSummaryDto> Wallets { get; set; } = new();
        public List<TransactionSummaryDto> RecentTransactions { get; set; } = new();
        public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
        public Dictionary<string, decimal> IncomeByCategory { get; set; } = new();
        public Dictionary<DateTime, decimal> BalanceHistory { get; set; } = new();
    }

    public class WalletSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = default!;
    }

    public class TransactionSummaryDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string WalletName { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public string TransactionTypeName => Type.ToString();
        public DateTime TransactionDate { get; set; }
        public string? Category { get; set; }
    }
}
