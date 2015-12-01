using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;

namespace Cliver.Fhr.ProductOffice.Models
{
    [MetadataType(typeof(CompanyAttributes))]
    public partial class Company
    {
    }

    sealed class CompanyAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Company")]
        public string Name { get; set; }

        [DataType(DataType.Url)]
        [Required(AllowEmptyStrings = false)]
        public string Url { get; set; }

        public string Comment { get; set; }

        [Display(Name = "Crawler")]
        public string CrawlerId { get; set; }
    }
}