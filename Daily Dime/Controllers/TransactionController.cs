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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<IActionResult> Index(int page =1)
        {
          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionRepository.GetAllAsync(userId);

            var pageSize = 5;
            var totalItems = transactions.Count();

            var pagedTransactions = transactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PagedTransactionViewModel
            {
                Transactions = pagedTransactions, 
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return View(viewModel);
        }


        [Authorize]
        public async Task<IActionResult> AddOrEdit(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve categories specific to the current user
            var userCategories = await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Populate the dropdown with user-specific categories
            ViewData["CategoryId"] = new SelectList(userCategories, "CategoryId", "TitleAndIcon");

            if (id == null || id == 0)
            {
                // Creating a new transaction
                return View(new Transaction());
            }

            // Editing existing transaction
            var transaction = await _transactionRepository.GetByIdAsync(id.Value, userId);
            if (transaction == null )
                //|| transaction.UserId != userId)
                return Forbid();




            return View(transaction);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("Category");

            if (transaction.Amount == 0)
            {
                ModelState.AddModelError("Amount", "Amount cannot be zero.");
            }


            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    Console.WriteLine($"Key: {state.Key}, Errors: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                var userCategories = await _context.Categories
      .Where(c => c.UserId == userId)
      .ToListAsync();
                transaction.Category = userCategories.FirstOrDefault(c => c.CategoryId == transaction.CategoryId);
                ViewData["CategoryId"] = new SelectList(userCategories, "CategoryId", "Title", transaction.CategoryId);
                return View(transaction);
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
                var existingTransaction = await _transactionRepository.GetByIdAsync(transaction.TransactionId, userId);
                if (existingTransaction == null || existingTransaction.UserId != userId)
                    return Forbid();

                // Update fields
                existingTransaction.CategoryId = transaction.CategoryId;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Note = transaction.Note;
                existingTransaction.Date = transaction.Date;

                await _transactionRepository.UpdateAsync(existingTransaction);
            }

            return RedirectToAction(nameof(Index));
        }





        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction == null )
                //|| transaction.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            await _transactionRepository.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }


        //private bool TransactionExists(int id)
        //{
        //    return _context.Transactions.Any(e => e.TransactionId == id);
        //}
    }
}
