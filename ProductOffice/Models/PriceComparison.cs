using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cliver.Fhr.ProductOffice.Models;

namespace Cliver.ProductOffice.Models
{
    public class PriceComparison
    {
        [Display(Name = "Product")]
        [Required]
        public Product Product { get; set; }

        [Display(Name = "Minimal difference (%)")]
        [Required()]
        public DateTime MinDifference { get; set; }

        [Display(Name = "Maximal difference (%)")]
        [Required()]
        public DateTime MaxDifference { get; set; }

        [Display(Name = "Avarage difference (%)")]
        [Required()]
        public DateTime AvarageDifference { get; set; }
    }
}