using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;

namespace Cliver.FhrApi.ProductOffice.Models
{
    [MetadataType(typeof(ProductAttributes))]
    public partial class Product
    {
    }

    sealed class ProductAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Company")]
        [Required(AllowEmptyStrings = false)]
        public int CompanyId { get; set; }

        [Display(Name = "Company Category")]
        public string Category { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Id in Company")]
        public string ExternalId { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreateTime { get; set; }

        [Required]
        [Display(Name = "Modification Time")]
        public DateTime ModifyTime { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [DataType(DataType.Url)]
        //[Required(AllowEmptyStrings = false)]
        public string Url { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [Display(Name = "Image urls separated by new line")]
        public string ImageUrls { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string Sku { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        public int Source { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [Display(Name = "Link Id")]
        public int LinkId { get; set; }        
    }
}