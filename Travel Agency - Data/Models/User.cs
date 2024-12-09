using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.Models
{
    public class User: IdentityUser
    {

        public string? FullName { get; set; }

        public int? CustomerId { get; set; }

        public Customer customer { get; set; }
    }
}
