using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using WalletManager.Application.DTOs;
using WalletManager.Application.Services.Interfaces;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Interfaces;

namespace WalletManager.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WalletDto>> GetAllWalletsAsync(string userId)
        {
            var wallets = await _unitOfWork.Wallets.GetWalletsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<WalletDto>>(wallets);
        }

        public async Task<WalletDto?> GetWalletByIdAsync(int id, string userId)
        {
            var wallet = await _unitOfWork.Wallets.GetWalletWithTransactionsAsync(id);
            if (wallet == null || wallet.UserId != userId)
            {
                return null;
            }
            
            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<WalletDto> CreateWalletAsync(CreateWalletDto createWalletDto, string userId)
        {
            var wallet = _mapper.Map<Wallet>(createWalletDto);
            wallet.UserId = userId;

            await _unitOfWork.Wallets.AddAsync(wallet);
            await _unitOfWork.SaveChangesAsync();

            // If initial balance is greater than 0, create an income transaction
            if (createWalletDto.InitialBalance > 0)
            {
                var transaction = new Transaction
                {
                    WalletId = wallet.Id,
                    Amount = createWalletDto.InitialBalance,
                    Description = "Initial balance",
                    Type = Domain.Enums.TransactionType.Income,
                    TransactionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Category = "Initial"
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<WalletDto?> UpdateWalletAsync(int id, UpdateWalletDto updateWalletDto, string userId)
        {
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(id);
            if (wallet == null || wallet.UserId != userId)
            {
                return null;
            }

            _mapper.Map(updateWalletDto, wallet);
            _unitOfWork.Wallets.Update(wallet);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<bool> DeleteWalletAsync(int id, string userId)
        {
            var wallet = await _unitOfWork.Wallets.GetByIdAsync(id);
            if (wallet == null || wallet.UserId != userId)
            {
                return false;
            }

            // Get all transactions for this wallet
            var transactions = await _unitOfWork.Transactions.GetTransactionsByWalletIdAsync(id);
            foreach (var transaction in transactions)
            {
                _unitOfWork.Transactions.Delete(transaction);
            }

            _unitOfWork.Wallets.Delete(wallet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetTotalBalanceAsync(string userId)
        {
            return await _unitOfWork.Wallets.GetTotalBalanceByUserIdAsync(userId);
        }
    }
}
