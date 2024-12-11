using System.ComponentModel.DataAnnotations;

namespace Travel_Agency___Data.ViewModels
{
    public class ProfileViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "First Name cannot be longer than 25 characters.")]
        public string CustFirstName { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Last Name cannot be longer than 25 characters.")]
        public string CustLastName { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "Address cannot be longer than 75 characters.")]
        public string CustAddress { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters.")]
        public string CustCity { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "Province must be 2 characters.")]
        public string CustProv { get; set; }

        [Required]
        [StringLength(7, ErrorMessage = "Postal code must be 7 characters.")]
        public string CustPostal { get; set; }

        [StringLength(25)]
        public string? CustCountry { get; set; }

        [StringLength(20)]
        public string? CustHomePhone { get; set; }

        [Required]
        [StringLength(20)]
        public string CustBusPhone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string CustEmail { get; set; }
    }
}
