using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDataAccess.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //fk
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        // Foreign key to IdentityUser
        [Required]
      
        public string UserId { get; set; }

        // Navigation property (optional)
        public IdentityUser User { get; set; }
        // public int CategoryId { get; set; }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Expense") ? "- " : "+ ") + Amount.ToString("C2");
            }
        }
    }
}
