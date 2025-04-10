using System.Text.Json.Serialization;

namespace WalletManager.Blazor.Models
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TransactionCount { get; set; }
    }

    public class CreateWalletDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal InitialBalance { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class UpdateWalletDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Currency { get; set; } = "USD";
    }

    public class WalletSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = default!;
    }
}
