using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Travel_Agency___Data.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }

        public int? CustomerId { get; set; }

        public Customer Customer { get; set; }

        public string ProfilePicture { get; set; } 
    }
}
