using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cliver.ProductOffice.Models
{
    public class Import
    {
        [Display(Name = "Importing product file")]
        [Required(ErrorMessage = "Specify an importing file")]
        //[FileExtensions(Extensions = "xls,xlsx", ErrorMessage = "Please upload valid format")]
        [UploadingFile]
        public HttpPostedFileBase File { get; set; }

        [Display(Name = "Product actual date")]
        [Required()]
        public DateTime UpdateTime { get; set; }

        [Display(Name = "Import map between file columns and product properties")]
        [Required(AllowEmptyStrings = false)]
        public int MapId { get; set; }

        [Display(Name = "Only check file and not import")]
        public bool CheckNotImport { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UploadingFileAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,  ValidationContext validationContext)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;

            if (file == null)  
                return new ValidationResult("Please upload a file!");

            //if (file.ContentLength > 10 * 1024 * 1024)
            //    return new ValidationResult("This file is too big!");

            if (!Regex.IsMatch(file.FileName, @"\.(xls|xlsx)$", RegexOptions.IgnoreCase))
                return new ValidationResult("The file has unsupported format!");

            return ValidationResult.Success;
        }
    }
}