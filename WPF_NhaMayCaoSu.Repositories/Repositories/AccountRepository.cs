﻿using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private CaoSuWpfDbContext _context;

        public async Task CreateAccountAsync(Account account)
        {
            _context = new();
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            _context = new();
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.Status = 0; //0 is unavailable 
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Account> GetAccountByIdAsync(Guid accountId)
        {
            _context = new();
            return await _context.Accounts
                                 .Include(a => a.Role)
                                 .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync(int pageNumber, int pageSize)
        {
            _context = new();
            return await _context.Accounts
                                 .Include(a => a.Role)
                                 .Where(a => a.Status == 1)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<Account> Login(string username, string password)
        {
            _context = new();
            var account = await _context.Accounts
                                        .FirstOrDefaultAsync(a => a.Username.ToLower().Equals(username));

            if (account != null && BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return account;
            }

            return null; 
        }

        public async Task Register(Account account)
        {
            _context = new();
            await CreateAccountAsync(account);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context = new();
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
