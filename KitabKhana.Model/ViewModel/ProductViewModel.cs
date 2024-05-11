using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KitabKhana.Model.Base;



namespace KitabKhana.Model.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }        
        public string? Author { get; set; }
        public double ListPrice { get; set; }
        public double Price100 { get; set; }
        public string? ImageUrl { get; set; }    
        public Product Product { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; } 
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CoverList { get; set; }

        public string GenreTest { get; set; }   
        public IEnumerable<SelectListItem> GenreList { get; set; }
    }
}
