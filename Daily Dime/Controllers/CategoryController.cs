using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Daily_Dime.Data;
using FTDataAccess.Models;
using FTDataAccess.Repository;
using FTDataAccess.Interface;
using System.Security.Claims;
using Syncfusion.EJ2.Navigations;
using Daily_Dime.Models;
using Microsoft.AspNetCore.Authorization;


namespace Daily_Dime.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryRepo;


        public CategoryController(ICategory categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }


        [Authorize]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pageSize = 5; // Items per page

            var allCategories = await _categoryRepo.GetAllAsync(userId);
            var totalItems = allCategories.Count();

            var categories = allCategories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PagedCategoryViewModel
            {
                Categories = categories,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

     

            return View(viewModel);
        }






        // GET: Category/Details/5
     
        [Authorize]
        public async Task<IActionResult> AddOrEdit(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null || id == 0)
            {
                // Creating a new category
                return View(new Category());
            }

            // Editing existing category
            var category = await _categoryRepo.GetByIdAsync(id.Value, userId);
            if (category == null)
                return NotFound();

          

            return View(category);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Category category)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
                return View(category);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (category.CategoryId == 0)
            {
                // Creating new category
                category.UserId = userId;
                await _categoryRepo.AddAsync(category, userId);
            }
            else
            {
                // Editing existing category
                var existingCategory = await _categoryRepo.GetByIdAsync(category.CategoryId, userId);
                if (existingCategory == null)
                    return NotFound();

                // Update only the editable fields
                existingCategory.Title = category.Title;
                existingCategory.Icon = category.Icon;
                existingCategory.Type = category.Type;

                await _categoryRepo.UpdateAsync(existingCategory, userId);
            }

            return RedirectToAction(nameof(Index));
        }




        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryRepo.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }





  
}


}

