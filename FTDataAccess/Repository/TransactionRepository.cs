﻿using Daily_Dime.Data;
using FTDataAccess.Interface;
using FTDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDataAccess.Repository
{
   public class TransactionRepository : ITransaction
    {

        private readonly ApplicationDbContext _context;

        public TransactionRepository (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync(string userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }



        public async Task<Transaction> GetByIdAsync(int id, string userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.UserId == userId);
        }
        public async Task AddAsync(Transaction transaction, string userId)
        {
            transaction.UserId = userId;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

       
        public async Task DeleteAsync(int id, string userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.UserId == userId);

            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
     

        public async Task<bool> ExistsAsync(int id, string userId)
        {
            return await _context.Transactions
                .AnyAsync(t => t.TransactionId == id && t.UserId == userId);
        }









    }
}
