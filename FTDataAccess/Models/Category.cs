﻿using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDataAccess.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Icon { get; set; } = "";
        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Expense";

        [Required]
     
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }


        [NotMapped]
        public string TitleAndIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }
    }
}
