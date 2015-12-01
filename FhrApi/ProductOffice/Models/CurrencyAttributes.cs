using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;

namespace Cliver.Fhr.ProductOffice.Models
{
    [MetadataType(typeof(CurrencyAttributes))]
    public partial class Currency
    {
    }

    sealed class CurrencyAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Currency")]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Display(Name = "Currency Sign")]
        [Required(AllowEmptyStrings = false)]
        public string Symbol { get; set; }
    }
}