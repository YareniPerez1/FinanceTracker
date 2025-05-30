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

        

        Task<IEnumerable<Transaction>> GetAllAsync(string UserId);
      
        Task<Transaction?> GetByIdAsync(int id, string userId);
        Task AddAsync(Transaction transactiony, string userId);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(int id, string userId);
        Task<bool> ExistsAsync(int id, string userId);
    }
}
