using System;
using System.ComponentModel.DataAnnotations;

namespace WalletManager.Application.DTOs
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal InitialBalance { get; set; }
        public string Currency { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TransactionCount { get; set; }
    }

    public class CreateWalletDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = default!;
        
        [StringLength(200)]
        public string Description { get; set; } = default!;
        
        [Required]
        public decimal InitialBalance { get; set; } = 0;
        
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "USD";
    }

    public class UpdateWalletDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = default!;
        
        [StringLength(200)]
        public string Description { get; set; } = default!;
        
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "USD";
    }
}
