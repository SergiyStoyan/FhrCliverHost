using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;

namespace Cliver.ProductOffice.Models
{
    [MetadataType(typeof(ImportMapAttributes))]
    public partial class ImportMap
    {
    }

    sealed class ImportMapAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Map")]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Display(Name = "Skip First Row")]
        [Required(AllowEmptyStrings = false)]
        public bool SkipFirstRow { get; set; }

        [Display(Name = "Company")]
        [Required(AllowEmptyStrings = false)]
        public int CompanyId { get; set; }

        [Display(Name = "Currency")]
        [Required(AllowEmptyStrings = false)]
        public int CurrencyId { get; set; }

        [Display(Name = "Company Product Id")]
        [Required(AllowEmptyStrings = false)]
        public int C_CompanyProductIdI { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        public Nullable<int> C_NameI { get; set; }

        [Display(Name = "Sku")]
        public Nullable<int> C_SkuI { get; set; }

        [Display(Name = "Price")]
        public Nullable<int> C_PriceI { get; set; }

        [Display(Name = "Description")]
        public Nullable<int> C_DescriptionI { get; set; }

        [Display(Name = "Category")]
        public Nullable<int> C_CategoryI { get; set; }
    }
}