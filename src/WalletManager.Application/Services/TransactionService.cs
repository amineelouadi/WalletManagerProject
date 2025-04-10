using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Enums;
using WalletManager.Domain.Interfaces;

namespace WalletManager.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(string userId)
        {
            var transactions = await _unitOfWork.Transactions.GetTransactionsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByWalletIdAsync(int walletId, string userId)
        {
            // First check if wallet belongs to user
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(walletId);
            if (wallet == null || wallet.UserId != userId)
            {
                return new List<TransactionDto>();
            }

            var transactions = await _unitOfWork.Transactions.GetTransactionsByWalletIdAsync(walletId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id, string userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
            if (transaction == null)
            {
                return null;
            }

            // Check if the wallet belongs to the user
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(transaction.WalletId);
            if (wallet == null || wallet.UserId != userId)
            {
                return null;
            }

            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createTransactionDto, string userId)
        {
            // Verify that wallet exists and belongs to user
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(createTransactionDto.WalletId);
            if (wallet == null || wallet.UserId != userId)
            {
                throw new UnauthorizedAccessException("You don't have access to this wallet.");
            }

            var transaction = _mapper.Map<Transaction>(createTransactionDto);

            // Update wallet balance
            if (transaction.Type == TransactionType.Income)
            {
                wallet.Balance += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Expense)
            {
                wallet.Balance -= transaction.Amount;
            }
            
            // Set last updated time for wallet
            wallet.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Transactions.AddAsync(transaction);
            _unitOfWork.Wallets.Update(wallet);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto updateTransactionDto, string userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
            if (transaction == null)
            {
                return null;
            }

            // Verify that wallet belongs to user
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(transaction.WalletId);
            if (wallet == null || wallet.UserId != userId)
            {
                return null;
            }

            // Calculate balance adjustment
            decimal balanceAdjustment = 0;
            
            // Reverse the effect of the old transaction
            if (transaction.Type == TransactionType.Income)
            {
                balanceAdjustment -= transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Expense)
            {
                balanceAdjustment += transaction.Amount;
            }

            // Apply the new transaction
            if (updateTransactionDto.Type == TransactionType.Income)
            {
                balanceAdjustment += updateTransactionDto.Amount;
            }
            else if (updateTransactionDto.Type == TransactionType.Expense)
            {
                balanceAdjustment -= updateTransactionDto.Amount;
            }

            // Update transaction
            _mapper.Map(updateTransactionDto, transaction);
            
            // Update wallet balance
            wallet.Balance += balanceAdjustment;
            wallet.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Transactions.Update(transaction);
            _unitOfWork.Wallets.Update(wallet);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id, string userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
            if (transaction == null)
            {
                return false;
            }

            // Verify that wallet belongs to user
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(transaction.WalletId);
            if (wallet == null || wallet.UserId != userId)
            {
                return false;
            }

            // Update wallet balance
            if (transaction.Type == TransactionType.Income)
            {
                wallet.Balance -= transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Expense)
            {
                wallet.Balance += transaction.Amount;
            }
            
            wallet.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Transactions.Delete(transaction);
            _unitOfWork.Wallets.Update(wallet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, string userId)
        {
            var transactions = await _unitOfWork.Transactions.GetTransactionsByDateRangeAsync(startDate, endDate, userId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByTypeAsync(TransactionType type, string userId)
        {
            var transactions = await _unitOfWork.Transactions.GetTransactionsByTypeAsync(type, userId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<decimal> GetSumByTypeAndDateRangeAsync(TransactionType type, DateTime startDate, DateTime endDate, string userId)
        {
            return await _unitOfWork.Transactions.GetSumByTypeAndDateRangeAsync(type, startDate, endDate, userId);
        }

        public async Task<DashboardDto> GetDashboardDataAsync(string userId)
        {
            // Get user wallets
            var wallets = await _unitOfWork.Wallets.GetWalletsByUserIdAsync(userId);
            var walletDtos = _mapper.Map<List<WalletSummaryDto>>(wallets);

            // Calculate total balance
            var totalBalance = wallets.Sum(w => w.Balance);

            // Get recent transactions (last 10)
            var allTransactions = await _unitOfWork.Transactions.GetTransactionsByUserIdAsync(userId);
            var recentTransactions = allTransactions
                .OrderByDescending(t => t.TransactionDate)
                .Take(10)
                .ToList();

            var recentTransactionDtos = _mapper.Map<List<TransactionSummaryDto>>(recentTransactions);

            // Get income and expense summaries for the current month
            var today = DateTime.UtcNow;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var incomeSum = await _unitOfWork.Transactions.GetSumByTypeAndDateRangeAsync(
                TransactionType.Income, startOfMonth, endOfMonth, userId);

            var expenseSum = await _unitOfWork.Transactions.GetSumByTypeAndDateRangeAsync(
                TransactionType.Expense, startOfMonth, endOfMonth, userId);

            // Get expenses by category
            var expensesByCategory = allTransactions
                .Where(t => t.Type == TransactionType.Expense && t.TransactionDate >= startOfMonth && t.TransactionDate <= endOfMonth && !string.IsNullOrEmpty(t.Category))
                .GroupBy(t => t.Category!)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            // Get income by category
            var incomeByCategory = allTransactions
                .Where(t => t.Type == TransactionType.Income && t.TransactionDate >= startOfMonth && t.TransactionDate <= endOfMonth && !string.IsNullOrEmpty(t.Category))
                .GroupBy(t => t.Category!)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            // Get balance history (last 30 days)
            var balanceHistory = new Dictionary<DateTime, decimal>();
            var startDate = today.AddDays(-30);
            
            for (var date = startDate; date <= today; date = date.AddDays(1))
            {
                var dayEnd = date.Date.AddDays(1).AddTicks(-1);
                var incomeForDay = allTransactions
                    .Where(t => t.Type == TransactionType.Income && t.TransactionDate.Date == date.Date)
                    .Sum(t => t.Amount);
                
                var expenseForDay = allTransactions
                    .Where(t => t.Type == TransactionType.Expense && t.TransactionDate.Date == date.Date)
                    .Sum(t => t.Amount);
                
                var netForDay = incomeForDay - expenseForDay;
                balanceHistory.Add(date.Date, netForDay);
            }

            return new DashboardDto
            {
                TotalBalance = totalBalance,
                WalletCount = wallets.Count(),
                TotalIncome = incomeSum,
                TotalExpense = expenseSum,
                Wallets = walletDtos,
                RecentTransactions = recentTransactionDtos,
                ExpensesByCategory = expensesByCategory,
                IncomeByCategory = incomeByCategory,
                BalanceHistory = balanceHistory
            };
        }
    }
}
