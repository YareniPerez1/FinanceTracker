using FTDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDataAccess.Interface
{
    public interface ITransaction
    {

        //Task<IEnumerable<Transaction>> GetAllAsync();
        //Task<Transaction> GetByIdAsync(int id);
        //Task AddAsync(Transaction transaction);
        //Task UpdateAsync(Transaction transaction);
        //Task DeleteAsync(int id);

        Task<IEnumerable<Transaction>> GetAllAsync(string UserId);
       // Task<List<Transaction>> GetAllAsync(string UserId);
        Task<Transaction?> GetByIdAsync(int id);
        Task AddAsync(Transaction transactiony, string userId); // Add userId parameter
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
