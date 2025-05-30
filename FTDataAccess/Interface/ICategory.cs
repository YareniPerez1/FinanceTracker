using FTDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDataAccess.Interface
{
    public interface ICategory
    {
        Task<IEnumerable<Category>> GetAllAsync(string UserId);
        Task<Category?> GetByIdAsync(int id, string userId);
        Task AddAsync(Category category, string userId); 
        Task UpdateAsync(Category category, string userId);
        Task DeleteAsync(int id, string userId);
        Task<bool> ExistsAsync(int id, string userId);
    }

}
