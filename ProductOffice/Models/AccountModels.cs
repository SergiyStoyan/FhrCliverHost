using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using WebMatrix.WebData;
using System.Linq;
//using Microsoft.AspNet.Identity;

namespace Cliver.ProductOffice.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base(Fhr.ProductOffice.Models.DbApi.GetProviderConnectionString())
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "User name")]
        [UserName]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Enabled")]
        [UserActive]
        public bool Active { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UserNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Please set a name!");

            string name = (string)value;

            bool invalid = false;
            UsersContext db = new UsersContext();
            if (validationContext.ObjectInstance is RegisterModel)
            {
                if (0 < db.UserProfiles.Where(r => r.UserName.ToLower() == ((RegisterModel)validationContext.ObjectInstance).UserName.ToLower()).Count())
                    invalid = true;
            }
            else
                if (validationContext.ObjectInstance is UserProfile)
                {
                    UserProfile up = (UserProfile)validationContext.ObjectInstance;
                    if (0 < db.UserProfiles.Where(r => r.UserName.ToLower() == up.UserName.ToLower() && r.UserId != up.UserId).Count())
                        invalid = true;
                }
                else
                    throw new Exception("validationContext is not acceptable type.");
            if (invalid)
                return new ValidationResult("Such a name exists already. Please choose another one.");

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UserActiveAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Please set a value!");

            bool active = (bool)value;

            bool invalid = false;
            UsersContext db = new UsersContext();
            if (validationContext.ObjectInstance is RegisterModel)
            {
            }
            else
                if (validationContext.ObjectInstance is UserProfile)
                {
                    UserProfile up = (UserProfile)validationContext.ObjectInstance;
                    //WebSecurity.InitializeDatabaseConnection()
                    if (up.UserId == WebSecurity.CurrentUserId && !up.Active)
                        invalid = true;
                }
                else
                    throw new Exception("validationContext is not acceptable type.");
            if (invalid)
                return new ValidationResult("It is impossible to set own account inactive.");

            return ValidationResult.Success;
        }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        [UserName]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Enabled")]
        [UserActive]
        public bool Active { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
