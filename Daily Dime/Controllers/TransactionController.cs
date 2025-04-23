using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Daily_Dime.Data;
using FTDataAccess.Models;
using FTDataAccess.Interface;
using FTDataAccess.Repository;
using System.Security.Claims;

namespace Daily_Dime.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransaction _transactionRepository;
        private readonly ApplicationDbContext _context;

        public TransactionController(ITransaction transactionRepository, ApplicationDbContext context)
        {
            _transactionRepository = transactionRepository;
           _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index(int page =1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionRepository.GetAllAsync(userId);

            var pageSize = 5; // Items per page

          
            var totalItems = transactions.Count();

            var categories = transactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PagedTransactionViewModel
            {
                Transactions = transactions,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return View(viewModel);
        }

        // GET: Transaction/Create
        //public IActionResult AddOrEdit()
        //{
        //    ViewData["CatgoryId"] = new SelectList(_context.Categories, "CategoryId", "Icon");
        //    return View();
        //}

        //// POST: Transaction/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Transaction transaction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        await _transactionRepository.AddAsync(transaction, userId);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CatgoryId"] = new SelectList(_context.Categories, "CategoryId", "Icon", transaction.CatgoryId);
        //    return View(transaction);
        //  }

        //public async Task<IActionResult> AddOrEdit(int? id)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // Populate categories for the dropdown
        //    ViewData["CatgoryId"] = new SelectList(_context.Categories, "CategoryId", "Icon");


        //    if (id == null || id == 0)
        //    {
        //        // Creating a new transaction
        //        return View(new Transaction());
        //    }

        //    // Editing existing transaction
        //    var transaction = await _transactionRepository.GetByIdAsync(id.Value);
        //    if (transaction == null || transaction.UserId != userId)
        //        return NotFound();

        //    return View(transaction);
        //}

        public async Task<IActionResult> AddOrEdit(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve categories specific to the current user
            var userCategories = await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Populate the dropdown with user-specific categories
            ViewData["CategoryId"] = new SelectList(userCategories, "CategoryId", "Title");

            if (id == null || id == 0)
            {
                // Creating a new transaction
                return View(new Transaction());
            }

            // Editing existing transaction
            var transaction = await _transactionRepository.GetByIdAsync(id.Value);
            if (transaction == null || transaction.UserId != userId)
                return NotFound();

          


            return View(transaction);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CatgoryId,Amount,Note,Date")] Transaction transaction)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // Remove validation for navigation properties if necessary
        //    ModelState.Remove("User");
        //    ModelState.Remove("UserId");

        //    if (!ModelState.IsValid)
        //    {
        //        ViewData["CatgoryId"] = new SelectList(_context.Categories, "CategoryId", "Icon", transaction.CatgoryId);
        //        return View(transaction);
        //    }

        //    if (transaction.TransactionId == 0)
        //    {
        //        // Creating new transaction
        //        transaction.UserId = userId;
        //        await _transactionRepository.AddAsync(transaction, userId);
        //    }
        //    else
        //    {
        //        // Editing existing transaction
        //        var existingTransaction = await _transactionRepository.GetByIdAsync(transaction.TransactionId);
        //        if (existingTransaction == null || existingTransaction.UserId != userId)
        //            return NotFound();

        //        // Update fields
        //        existingTransaction.CatgoryId = transaction.CatgoryId;
        //        existingTransaction.Amount = transaction.Amount;
        //        existingTransaction.Note = transaction.Note;
        //        existingTransaction.Date = transaction.Date;

        //        await _transactionRepository.UpdateAsync(existingTransaction);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Remove validation for navigation properties if necessary
            ModelState.Remove("User");
            ModelState.Remove("UserId");
           // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (!ModelState.IsValid)
            //{
            //    var userCategories = await _context.Categories
            //        .Where(c => c.UserId == userId)
            //        .ToListAsync();
            //    ViewData["CategoryId"] = new SelectList(userCategories, "CategoryId", "Title", transaction.CategoryId);
            //    return View(transaction);
            //}
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    Console.WriteLine($"Key: {state.Key}, Errors: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            if (transaction.TransactionId == 0)
            {
                // Creating new transaction
                transaction.UserId = userId;
                await _transactionRepository.AddAsync(transaction, userId);
            }
            else
            {
                // Editing existing transaction
                var existingTransaction = await _transactionRepository.GetByIdAsync(transaction.TransactionId);
                if (existingTransaction == null || existingTransaction.UserId != userId)
                    return NotFound();

                // Update fields
                existingTransaction.CategoryId = transaction.CategoryId;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Note = transaction.Note;
                existingTransaction.Date = transaction.Date;

                await _transactionRepository.UpdateAsync(existingTransaction);
            }

            return RedirectToAction(nameof(Index));
        }


        //// GET: Transaction/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _transactionRepository.GetByIdAsync(id.Value);
        //    if (transaction == null || transaction.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        //    {
        //        return NotFound();
        //    }

        //    ViewData["CatgoryId"] = new SelectList(_context.Categories, "CategoryId", "Icon", transaction.CatgoryId);
        //    return View(transaction);
        //}

        //// POST: Transaction/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Transaction transaction)
        //{
        //    if (id != transaction.TransactionId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        transaction.UserId = userId;
        //        await _transactionRepository.UpdateAsync(transaction);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CatgoryId"] = new SelectList(_context.Categories, "CategoryId", "Icon", transaction.CatgoryId);
        //    return View(transaction);
        //}



        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null || transaction.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            await _transactionRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        //private bool TransactionExists(int id)
        //{
        //    return _context.Transactions.Any(e => e.TransactionId == id);
        //}
    }
}
