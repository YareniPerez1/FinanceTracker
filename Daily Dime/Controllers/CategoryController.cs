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

namespace Daily_Dime.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryRepo;


        public CategoryController(ICategory categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return View(categories);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryRepo.GetByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Category/Create
        //public IActionResult Create()
        //{
        //    return View(new Category());
        //}

        public async Task<IActionResult> AddOrEdit(int? id)
        {
            if (id == null || id == 0)
            {
                // Creating a new category
                return View(new Category());
            }

            // Editing existing category
            var category = await _categoryRepo.GetByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Category category)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    category.UserId = userId;

        //    ModelState.Remove("User"); // if navigation property exists
        //    ModelState.Remove("UserId"); // optional

        //    if (ModelState.IsValid)
        //    {
        //        await _categoryRepo.AddAsync(category, userId);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(category);
        //}


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
                var existingCategory = await _categoryRepo.GetByIdAsync(category.CategoryId);
                if (existingCategory == null)
                    return NotFound();

                // Update only the editable fields
                existingCategory.Title = category.Title;
                existingCategory.Icon = category.Icon;
                existingCategory.Type = category.Type;

                await _categoryRepo.UpdateAsync(existingCategory);
            }

            return RedirectToAction(nameof(Index));
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CategoryId,Title,Icon,Type")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the current logged-in user's ID
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get UserId from claims

        //        // Pass UserId to the repository when adding a new category
        //        await _categoryRepo.AddAsync(category, userId);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}
        //public async Task<IActionResult> Create([Bind("CategoryId,Title,Icon,Type")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _categoryRepo.AddAsync(category);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        // GET: Category/Edit/5
        //public async Task<IActionResult> AddOrEdit(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var category = await _categoryRepo.GetByIdAsync(id.Value);
        //    if (category == null)
        //        return NotFound();

        //    return View(category);
        //}

        // POST: Category/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Title,Icon,Type")] Category category)
        //{
        //    if (id != category.CategoryId)
        //        return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _categoryRepo.UpdateAsync(category);
        //        }
        //        catch
        //        {
        //            if (!await _categoryRepo.ExistsAsync(category.CategoryId))
        //                return NotFound();
        //            throw;
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit(int id, [Bind("CategoryId,Title,Icon,Type")] Category category)
        //{
        //    if (id != category.CategoryId)
        //        return NotFound();

        //    // Get current category from DB to preserve UserId
        //    var existingCategory = await _categoryRepo.GetByIdAsync(id);
        //    if (existingCategory == null)
        //        return NotFound();

        //    // Assign the existing UserId to avoid validation errors
        //    category.UserId = existingCategory.UserId;

        //    ModelState.Remove("User"); // prevent validation on navigation property
        //    ModelState.Remove("UserId"); // needed if UserId is [Required]

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _categoryRepo.UpdateAsync(category);
        //        }
        //        catch
        //        {
        //            if (!await _categoryRepo.ExistsAsync(category.CategoryId))
        //                return NotFound();
        //            throw;
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(category);
        //}


        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryRepo.GetByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }


}

