using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string? CustFirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? CustLastName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string? CustAddress { get; set; }

        [Required]
        [Display(Name = "City")]
        public string? CustCity { get; set; }

        [Required]
        [Display(Name = "Province")]
        public string? CustProv { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$", ErrorMessage = "Invalid Canadian postal code.")]
        public string? CustPostal { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string? CustCountry { get; set; }

        [Required]
        [Display(Name = "Home Phone")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number format.")]
        [DisplayFormat(DataFormatString = "{0:(###) ###-####}", ApplyFormatInEditMode = true)]
        public string? CustHomePhone { get; set; }

        [Required]
        [Display(Name = "Business Phone")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number format.")]
        [DisplayFormat(DataFormatString = "{0:(###) ###-####}", ApplyFormatInEditMode = true)]
        public string? CustBusPhone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? CustEmail { get; set; }

        public int? AgentId { get; set; }

        public List<Agent>? Agents { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
