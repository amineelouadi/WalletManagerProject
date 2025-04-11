using System.Text.Json.Serialization;

namespace WalletManager.Blazor.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Income = 1,
        Expense = 2,
        Transfer = 3
    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string WalletName { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public string TransactionTypeName { get; set; } = default!;
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }
        public string? Reference { get; set; }
    }

    public class CreateTransactionDto
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string? Category { get; set; }
        public string? Reference { get; set; }
    }

    public class UpdateTransactionDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Category { get; set; }
        public string? Reference { get; set; }
    }

    public class TransactionSummaryDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string WalletName { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public string TransactionTypeName { get; set; } = default!;
        public DateTime TransactionDate { get; set; }
        public string? Category { get; set; }
    }

    public class DashboardDto
    {
        public decimal TotalBalance { get; set; }
        public int WalletCount { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance { get; set; }
        public List<WalletSummaryDto> Wallets { get; set; } = new();
        public List<TransactionSummaryDto> RecentTransactions { get; set; } = new();
        public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
        public Dictionary<string, decimal> IncomeByCategory { get; set; } = new();
        public Dictionary<string, decimal> BalanceHistory { get; set; } = new();
    }
}
