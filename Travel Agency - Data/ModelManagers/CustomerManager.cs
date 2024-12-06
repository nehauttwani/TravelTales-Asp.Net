using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public class CustomerManager
    {
        private TravelExpertsContext _context { get; set; }

        public CustomerManager(TravelExpertsContext ctx)
        {
            _context = ctx;
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

        }

        public Customer GetCustomer(int id)
        {
            return _context.Customers.FirstOrDefault(x => x.CustomerId == id);
        }

    }
}
