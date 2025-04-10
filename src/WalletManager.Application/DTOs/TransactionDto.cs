using System;
using System.ComponentModel.DataAnnotations;
using WalletManager.Domain.Enums;

namespace WalletManager.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string WalletName { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public TransactionType Type { get; set; }
        public string TransactionTypeName => Type.ToString();
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }
        public string? Reference { get; set; }
    }

    public class CreateTransactionDto
    {
        [Required]
        public int WalletId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Description { get; set; } = default!;
        
        [Required]
        public TransactionType Type { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [StringLength(50)]
        public string? Reference { get; set; }
    }

    public class UpdateTransactionDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Description { get; set; } = default!;
        
        [Required]
        public TransactionType Type { get; set; }
        
        public DateTime TransactionDate { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [StringLength(50)]
        public string? Reference { get; set; }
    }
}
