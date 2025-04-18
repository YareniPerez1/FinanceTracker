using Daily_Dime.Data;
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
    public class CategoryRepository : ICategory
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category, string userId) // Accept userId as a parameter
        {
            category.UserId = userId; // Set the UserId of the category
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        //public async Task<IEnumerable<Category>> GetAllAsync()
        //{
        //    return await _context.Categories.ToListAsync();
        //}

        public async Task<IEnumerable<Category>> GetAllAsync(string userId)
        {
            return await _context.Categories
                                 .Where(c => c.UserId == userId)
                                 .ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

       

        public async Task UpdateAsync(Category category)
        {
            // Detach any existing entity with the same CategoryId to avoid tracking issues
            var existingCategory = await _context.Categories
                .AsNoTracking() // Prevents tracking while fetching the entity
                .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

            if (existingCategory == null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            // Detach any previously tracked instance of the same Category
            var trackedCategory = _context.ChangeTracker.Entries<Category>()
                                           .FirstOrDefault(c => c.Entity.CategoryId == category.CategoryId);
            if (trackedCategory != null)
            {
                trackedCategory.State = EntityState.Detached;
            }

            // Attach the new instance of the Category
            _context.Attach(category);

            // Mark the entity as modified
            _context.Entry(category).State = EntityState.Modified;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }




        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == id);
        }
    }

}
